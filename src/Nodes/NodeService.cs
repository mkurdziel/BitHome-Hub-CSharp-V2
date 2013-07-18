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

		public NodeService() 
		{
			log.Trace ("()");

			m_nodes = new Dictionary<string, Node> ();

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
            // Create the new node and give it a unique ID
			Node node = new Node ();
			node.Id = StorageService.GenerateKey ();

            // Save it in the lookup table
		    m_nodes[node.Id] = node;

			return node;
		}

        public Node[] GetNodes()
        {
            return m_nodes.Values.ToArray();
        }

        public Node GetNode(String p_key)
        {
            return m_nodes[p_key];
        }

		private void SaveNodeList() {
			StorageService.Store<String[]>.Insert (KEY_NODES, m_nodes.Keys.ToArray());
		}

	}
}

