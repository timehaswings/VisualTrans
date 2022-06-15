var ws = null;
var connected = false;
var screenSize = {width: 0, height: 0};
var tm = throttle(sendMousemove, 20);

window.onload = function(){
	console.log(Mime);
	openWs();
	document.onkeydown = handleKeydown;
	document.onkeyup = handleKeyup;
	var img = document.querySelector( "#photo" );
	img.oncontextmenu = handleNone;
	img.onmousemove = handleMousemove;
	img.onmousedown = handleMousedown;
	img.onmouseup = handleMouseup;
	img.onmousewheel = handleMousewheel;
	getScreenSize();
}

function openWs(){
	var img = document.querySelector( "#photo" );
	ws = new WebSocket('ws://'+ document.location.host + '/ws');
	ws.binaryType = "arraybuffer";
	
	ws.onopen = function(evt) { 
		console.log("Connection open ..."); 
	};
	
	ws.onmessage = function(evt) {
		var binaryData = [evt.data];
		var blob = new Blob(binaryData, {type: "image/jpeg"});
		var urlCreator = window.URL || window.webkitURL;
		var imageUrl = urlCreator.createObjectURL(blob);
		img.src = imageUrl;
	};
	
	ws.onclose = function(evt) {
		connected = false;
		console.log("Connection closed.");
	}; 
	
	ws.onerror = function(evt) {
		connected = false;
		console.log("Connection error." + evt.data);
	}
}

function start(){
	if(ws != null && ws.readyState == WebSocket.OPEN){
		ws.send("start");
		connected = true;
	}
}

function stop(){
	if(ws != null && ws.readyState == WebSocket.OPEN){
		ws.send("stop");
		connected = false;
	}
}
	
function closeWs(){
	if(ws != null & ws.readyState == WebSocket.OPEN){
		ws.close();
		ws = null;
	}
}

function enlarge(){
	document.querySelector( "#img-wrapper" ).classList.replace("normal-photo","large-photo");
	document.querySelector( "#minitool" ).style.display = 'block';
}

function restore(){
	document.querySelector( "#img-wrapper" ).classList.replace("large-photo","normal-photo");
	document.querySelector( "#minitool" ).style.display = 'none';
}

function handleNone(e){
	return false;
}

function handleKeydown(e) {
	if(connected){
		var keyNum = window.event ? e.keyCode:e.which;
		get(`/api/action/keydown/${keyNum}`);
	}
}

function handleKeyup(e){
	if(connected){
		var keyNum = window.event ? e.keyCode:e.which;
		get(`/api/action/keyup/${keyNum}`);
	}
}

function sendMousemove(e){
	var x = e.offsetX * screenSize.width / e.target.offsetWidth;
	var y = e.offsetY * screenSize.height / e.target.offsetHeight;
	get(`/api/action/move/${Math.round(x)}/${Math.round(y)}`);
}

function handleMousemove(e){
	if(connected){
		tm(e);
	}
}

function handleMousedown(e){
	if(connected){
		if(e.button === 0){
			get('/api/action/left/down');
		} else if (e.button === 1){
			get('/api/action/middle/down');
		} else if (e.button === 2){
			get('/api/action/right/down');
		}
	}
}

function handleMouseup(e){
	if(connected){
		if(e.button === 0){
			get('/api/action/left/up');
		} else if (e.button === 1){
			get('/api/action/middle/up');
		} else if (e.button === 2){
			get('/api/action/right/up');
		}
	}
}

function handleMousewheel(e){
	if(connected){
		get(`/api/action/scroll/${e.wheelDelta}`);
	}
}

function getScreenSize(){
	var cb = function(text){
		var res = JSON.parse(text);
		screenSize.width = res.width;
		screenSize.height = res.height;
	}
	get('/api/device/screen', cb);
}

function download(path){
	var pathInput = document.getElementById('pathInput');
	var data = { path: pathInput.value };
	var downloadCb = function(res){
		var pathArray = data.path.split("\\");
		var filename = pathArray[pathArray.length - 1];
		var ext = filename.split(".")[1];
		var mime = Mime._types[ext];
		var blob = new Blob([res.response], {type: mime});
		var objectUrl = URL.createObjectURL(blob)
		var a = document.createElement('a');
		a.setAttribute('href', objectUrl);
		a.setAttribute('download', filename);
		a.click();
		showToast('下载成功', `该文件已从远端下载：${pathInput.value}`);
	}
	var checkCb = function (text){
		var resObj = JSON.parse(text);
		if(resObj.msg === 'ok'){
			postArraybuffer('/api/device/download', data, downloadCb);
		} else {
			showToast('下载失败', resObj.msg);
		}
	}
	get(`/api/device/download/check/${encodeURIComponent(data.path)}`, checkCb);
}

function upload(){
	var files = document.getElementById("fileInput").files;
	var cb = function(res){
		showToast('上传成功', `文件所在远端目录：${res.responseText}`);
	}
	var form = new FormData();
	for(var i=0; i<files.length; i++){
		form.append("file"+i, files[i]);
	}
	postData("/api/device/upload", form, cb);
}

function showToast(title, content){
	var tipsToast = document.getElementById('tipsToast');
	var tipsToastTitle = document.getElementById('tipsToastTitle');
	var tipsToastBody = document.getElementById('tipsToastBody');
	tipsToastTitle.innerText = title;
	tipsToastBody.innerText = content;
	var toast = new bootstrap.Toast(tipsToast);
	toast.show();
}

function throttle(fn, delay) {
    var timer;
    return function(){
      if(!timer) {
        fn.apply(this, arguments)
        timer = setTimeout(()=>{
          clearTimeout(timer)
          timer = null
        },delay)
      }
    }
}

function get(url, callback=null){
	var xhr = new XMLHttpRequest();
	xhr.open('GET', url, true);
	xhr.send();
	xhr.onreadystatechange = function (e) {
		if (xhr.readyState == 4) {
			if(xhr.status == 200){
				if(callback){
					callback(xhr.responseText);
				}
			} else {
				console.error(xhr);
			}
		}
	}
}

function postArraybuffer(url, data, callback=null){
	var xhr = new XMLHttpRequest();
	xhr.open('POST', url, true);
	xhr.responseType = 'arraybuffer';
	xhr.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
	var dataStr = "";
	for(var key in data){
		dataStr += `${key}=${data[key]}&`;
	}
	var dataStr = dataStr.substr(0, dataStr.length - 1);
	xhr.send(dataStr);
	xhr.onreadystatechange = function (e) {
		if (xhr.readyState == 4) {
			if(xhr.status == 200){
				if(callback){
					callback(xhr);
				}
			} else {
				console.error(xhr);
			}
		}
	}
}

function postData(url, data, callback=null){
	var xhr = new XMLHttpRequest();
	xhr.open('POST', url, true);
	xhr.send(data);
	xhr.onreadystatechange = function (e) {
		if (xhr.readyState == 4) {
			if(xhr.status == 200){
				if(callback){
					callback(xhr);
				}
			} else {
				console.error(xhr);
			}
		}
	}
}