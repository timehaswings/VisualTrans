using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EmbedIO.WebSockets;
using VisualTrans.Driver;

namespace VisualTrans.Module
{
    class TransModule: WebSocketModule
    {
        private static Capture capture;
        private static HashSet<string> clients;
        private static BackgroundWorker worker;
        private static readonly int interval = 40;

        public TransModule(string urlPath)
            : base(urlPath, true)
        {
            if (capture == null)
            {
                capture = new Capture();
            }
            if (clients == null)
            {
                clients = new HashSet<string>();
            }
            if (worker == null)
            {
                worker = new BackgroundWorker
                {
                    WorkerSupportsCancellation = true
                };
                worker.DoWork += DoWorkEventHandler;
            }
            if (!worker.IsBusy)
            {
                worker.RunWorkerAsync();
            }
        }

        protected override Task OnMessageReceivedAsync(IWebSocketContext context, byte[] buffer, IWebSocketReceiveResult result)
        {
            var msg = Encoding.GetString(buffer);
            if (msg == "start")
            {
                clients.Add(context.Id);
            }
            else if (msg == "stop")
            {
                clients.Remove(context.Id);
            }
            else
            {
                return BroadcastAsync("instruction mismatch", c => c == context);
            }
            return BroadcastAsync("ok", c => c == context);
        }


        protected override Task OnClientConnectedAsync(IWebSocketContext context)
        {
            return BroadcastAsync("ok", c => c == context);
        }

        protected override Task OnClientDisconnectedAsync(IWebSocketContext context)
        {
            clients.Remove(context.Id);
            return BroadcastAsync("ok", c => c == context);
        }

        private void DoWorkEventHandler(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                if (clients.Count > 0)
                {
                    var screen = capture.GetScreen();
                    if (screen != null)
                    {
                        BroadcastAsync(screen, c => clients.Contains(c.Id));
                    }
                    Thread.Sleep(interval);
                }
                else
                {
                    Thread.Sleep(interval * 2);
                }
            }
        }
    }
}
