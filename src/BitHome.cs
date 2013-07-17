using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using ServiceStack.WebHost.Endpoints;

namespace BitHome
{
    static class BitHome
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        static int Main(String[] args)
        {
            var listeningOn = args.Length == 0 ? "http://*:1337/" : args[0];
            var appHost = new AppHost();
            appHost.Init();
            appHost.Start(listeningOn);

            log.Info("Bithome Created at {0}, listening on {1}", DateTime.Now, listeningOn);

            Console.ReadKey();

            return 0;
        }
    }

    public class AppHost : AppHostHttpListenerBase 
    {
        public AppHost() : base("BitHome Web Interface", typeof(BitHomeService).Assembly) { }

        public override void Configure(Funq.Container container)
        {
            Routes
                .Add<NodeBase>("/nodes/{id}")
                .Add<NodeBase>("/nodes");

            SetConfig(new EndpointHostConfig
            {
                DebugMode = true, //Show StackTraces for easier debugging (default auto inferred by Debug/Release builds)
            });
        }
    }
}
