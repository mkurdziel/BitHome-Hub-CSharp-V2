using System;
using System.Collections.Generic;
using System.Text;
using NLog;
using ServiceStack.WebHost.Endpoints;
using System.Threading;
using ServiceStack.ServiceHost;
using ServiceStack.Common;
using ServiceStack.Razor;

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
			Thread.Sleep(Timeout.Infinite);
        }
    }

    public class AppHost : AppHostHttpListenerBase 
    {
        public AppHost() : base("BitHome Web Interface", typeof(BitHomeService).Assembly) { }

        public override void Configure(Funq.Container container)
        {
//			ServiceStack.Logging.LogManager.LogFactory = new NLogFactory ();

			Plugins.Add(new RazorFormat());

            SetConfig(new EndpointHostConfig
            {
                //EnableFeatures = Feature.All.Remove(Feature.Metadata),
                DebugMode = true, //Show StackTraces for easier debugging (default auto inferred by Debug/Release builds)
            });
        }
    }
}
