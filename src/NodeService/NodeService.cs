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
	public class NodeService : ServiceStack.ServiceInterface.Service
	{
		private static Logger log = LogManager.GetCurrentClassLogger();

		public NodeService() {
			log.Trace ("()");

		}

		public bool Start() {
			log.Info ("Starting NodeService");

			
			NodeBase testNode = new NodeBase ();
			testNode.Name = "New Node";

			Db.DropAndCreateTable<NodeBase> ();
			Db.DropAndCreateTable<NodeXbee> ();

			NodeXbee testNodeXbee = new NodeXbee (64, 16);

			Db.Save<NodeBase> ();
			Db.Save<NodeXbee> ();

			Db.Insert<NodeBase> (testNode);
			Db.Insert<NodeXbee> (testNodeXbee);

			//			//Executes the specified delegate against the configured database.

			return true;
		}

		public void Stop() {
			log.Info ("Stopping NodeService");
		}

		public NodeBase GetNode (UInt64 id)
		{
			return Db.Id<NodeBase> (id);
		}
	}
}

