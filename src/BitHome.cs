using System;
using System.Collections.Generic;
using System.Text;
using NLog;
using ServiceStack.WebHost.Endpoints;
using System.Threading;
using ServiceStack.ServiceHost;
using ServiceStack.Common;
using ServiceStack.Razor;
using System.Net;
using ServiceStack.Common.Web;

namespace BitHome
{
    static class BitHome
    {

        private static Logger log = LogManager.GetLogger("BitHome");

        static void Main(String[] args)
        {
            var listeningOn = args.Length == 0 ? "http://*:1337/" : args[0];

            log.Info("Bithome Created at {0}, listening on {1}", DateTime.Now, listeningOn);

            var appHost = new AppHost();
            appHost.Init();
            appHost.Start(listeningOn);

			ServiceManager.Start ();

//			var proc = new System.Diagnostics.Process ();
//			proc.StartInfo.UseShellExecute = true;
//			proc.StartInfo.FileName = "http://localhost:1337/";
//			proc.Start ();

			Console.WriteLine("\n\nListening on http://*:1337/..");
			Console.WriteLine("Type Ctrl+C to quit..");

			while (ServiceManager.IsStarted) {
				Thread.Sleep (TimeSpan.FromSeconds(1));
			}

			appHost.Stop ();

			log.Info ("BitHome Shut Down");
        }
    }

    public class AppHost : AppHostHttpListenerBase 
    {
        public AppHost() : base("BitHome Web Interface", typeof(AppHost).Assembly) { }

        public override void Configure(Funq.Container container)
        {
//			ServiceStack.Logging.LogManager.LogFactory = new NLogFactory ();
			ServiceStack.Logging.LogManager.LogFactory = new ServiceStack.Logging.Support.Logging.ConsoleLogFactory();

			Plugins.Add(new RazorFormat());

			SetConfig(new EndpointHostConfig {
				DefaultContentType = ContentType.Json,
				CustomHttpHandlers = {
					{ HttpStatusCode.NotFound, new RazorHandler("/notfound") }
				}
			});
        }
    }
}
