using System;
using System.Net;
using System.Threading;
using Grapevine.Server;

namespace Motion
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            var server = new RESTServer(Config.Get("server_host"))
            {
                Port = Config.Get("server_port")
            };

            bool isRunning = true;
            Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e)
            {
                e.Cancel = true;
                isRunning = false;
            };

            server.Start();
            while (server.IsListening && isRunning)
            {
                Thread.Sleep(300);
            }
            server.Stop();
        }
    }

    public sealed class DefaultRouter : RESTResource
    {
        [RESTRoute]
        public void HandleAllGetRequests(HttpListenerContext context)
        {
            SendTextResponse(context, "Bad Request");
        }
    }
}
