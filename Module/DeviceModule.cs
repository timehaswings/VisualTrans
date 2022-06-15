using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using EmbedIO;
using EmbedIO.Routing;
using EmbedIO.WebApi;
using HttpMultipartParser;
using Swan.Logging;

namespace VisualTrans.Module
{
    class DeviceModule: WebApiController
    {

        private readonly string cacheDir;

        public DeviceModule() {
            cacheDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Cache");
            if (!Directory.Exists(cacheDir)) {
                Directory.CreateDirectory(cacheDir);
            }
        }

        [Route(HttpVerbs.Get, "/device/screen")]
        public object GetScreen()
        {
            var bounds = Screen.PrimaryScreen.Bounds;
            var w = bounds.Width;
            var h = bounds.Height;
            return new { width = w, height = h };
        }

        [Route(HttpVerbs.Get, "/device/download/check/{path}")]
        public object DownloadCheck(string path)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(path);
                if (fileInfo == null || !fileInfo.Exists)
                {
                    return new { msg = "文件不存在" };
                }
                if (fileInfo.Length > 200 * 1024 * 1024)
                {
                    return new { msg = "文件超过200M" };
                }
            }
            catch (Exception e)
            {
                return new { msg = e.Message };
            }
            return new { msg = "ok" };
        }

        [Route(HttpVerbs.Post, "/device/download")]
        public async Task Download([FormField] string path)
        {
            byte[] dataBuffer = File.ReadAllBytes(path);
            using var stream = HttpContext.OpenResponseStream();
            await stream.WriteAsync(dataBuffer, 0, dataBuffer.Length);
        }

        [Route(HttpVerbs.Post, "/device/upload")]
        public async Task Upload()
        {
            var parser = await MultipartFormDataParser.ParseAsync(Request.InputStream);
            foreach (var file in parser.Files) {
                var filepath = Path.Combine(cacheDir, file.FileName);
                using (var fs = File.Create(filepath))
                {
                    file.Data.Seek(0, SeekOrigin.Begin);
                    await file.Data.CopyToAsync(fs);
                }
            }
            using var writer = HttpContext.OpenResponseText();
            await writer.WriteAsync(cacheDir);
        }
    }
}
