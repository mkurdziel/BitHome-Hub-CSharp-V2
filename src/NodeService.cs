using System;
using System.Collections.Generic;
using System.Net;
using ServiceStack.Common;
using ServiceStack.Common.Web;
using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;
using ServiceStack.ServiceHost;
using ServiceStack.Text;
using NLog;

namespace BitHome
{
	public class NodeService 
	{
		private static Logger log = LogManager.GetCurrentClassLogger();

		public NodeService() {
			log.Trace ("()");
		}

		public bool Start() {
			log.Info ("Starting NodeService");

			return true;
		}

		public void Stop() {
			log.Info ("Stopping NodeService");
		}
	}
}

