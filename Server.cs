using System;
using System.IO;
using EmbedIO;
using EmbedIO.WebApi;
using Swan.Logging;
using VisualTrans.Module;

namespace VisualTrans
{
    class Server
    {
        public string Url { get; set; } = "http://+:9696/";
        private readonly string httpRoot;

        public Server() {
            httpRoot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Html");
        }
        
        public WebServer CreateWebServer()
        {
            TransModule transModule = new TransModule("/ws");
            var server = new WebServer(o => o
                    .WithUrlPrefix(Url)
                    .WithMode(HttpListenerMode.EmbedIO))
                    .WithLocalSessionManager()
                    .WithCors()
                    .WithWebApi("/api", m => m
                        .WithController<DeviceModule>()
                        .WithController<ActionModule>()
                    )
                    .WithModule(transModule)
                    .WithStaticFolder("/", httpRoot, true);

            server.StateChanged += (s, e) => Logger.Debug($"WebServer New State - {e.NewState}");

            server.HandleHttpException(async (context, exception) =>
            {
                context.Response.StatusCode = exception.StatusCode;
                switch (exception.StatusCode)
                {
                    case 404:
                        await context.SendDataAsync(new { success = false, message = "You are lost ..." });
                        break;
                    case 500:
                        await context.SendDataAsync(new { success = false, message = "Server internal error" });
                        break;
                    default:
                        Logger.Error(exception.Message);
                        await context.SendDataAsync(new { success = false, message = "Unknown exception" });
                        break;
                }
            });
            return server;
        }

    }
}
