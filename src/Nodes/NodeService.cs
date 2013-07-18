using System;
using System.Collections.Generic;
using System.Net;
using ServiceStack.Common;
using ServiceStack.Common.Web;
using ServiceStack.DataAnnotations;
using ServiceStack.ServiceHost;
using ServiceStack.Text;
using NLog;

namespace BitHome
{
	public class NodeService : ServiceStack.ServiceInterface.Service
	{
		const string KEY_NODES = "nodes";

		private static Logger log = LogManager.GetCurrentClassLogger();

		private Dictionary<String, Node> m_nodes;

		private Dictionary<UInt64, String> m_xbeeAddress64;

		public NodeService() 
		{
			log.Trace ("()");

			m_nodes = new Dictionary<string, Node> ();
			m_xbeeAddress64 = new Dictionary<UInt64, string>();

			// Load data from the storage service
			if (StorageService.Store<String[]>.Exists(KEY_NODES)) {
				String[] nodeKeys = StorageService.Store<String[]>.Get (KEY_NODES);

				foreach (String key in nodeKeys) 
				{
					if ( StorageService.Store<Node>.Exists(key) )
					{
						m_nodes.Add (key, StorageService.Store<Node>.Get (key));
					}
				}
			}
		}

		public bool Start() 
		{
			log.Info ("Starting NodeService");

			return true;
		}

		public void Stop() 
		{
			log.Info ("Stopping NodeService");
		}

		public Node CreateNode() 
		{
			Node node = new Node ();
			node.Id = StorageService.GenerateKey ();

			return node;
		}

		public Node GetNodeXbee (UInt64 address64, UInt16 address16, bool createIfNull)
		{
//			Node xbeeNode = m_xbeeNodes [address64];
//
//			if (createIfNull && xbeeNode == null) {
//				xbeeNode = CreateNode ();
//			}

			return null;
		}

		private void SaveNodeList() {
			StorageService.Store<String[]>.Insert (KEY_NODES, m_nodes.Keys.ToArray());
		}
	}
}

