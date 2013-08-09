using ServiceStack.ServiceHost;

namespace BitHome.WebApi
{
	[Route("/api/nodes", "GET")]
	public class WebApiNodes : IReturn<Node[]> { }

	[Route("/api/nodes/{NodeId}/name", "POST")]
	public class WebApiNodeName : IReturn<Node> {
        public string NodeId { get; set; }
        public string Name { get; set; }
    }

    [Route("/api/nodes/{NodeId}/reboot", "POST")]
	public class WebApiNodeReboot : IReturn<Node>
    {
        public string NodeId { get; set; }
    }

	public class NodeWebApiService : ServiceStack.ServiceInterface.Service
	{
		public NodeService NodeService {
			get {
				return ServiceManager.NodeService;
			}
		}

		public Node[] Get(WebApiNodes request) 
		{
			return NodeService.GetNodes ();
		}

		public Node Post(WebApiNodeName request)
        {
            Node node = NodeService.GetNode(request.NodeId);

            if (node!=null)
            {
                NodeService.SetNodeName(node.Id, request.Name);
            }

            return node;
        }


		public Node Post(WebApiNodeReboot request)
        {
            Node node = NodeService.GetNode(request.NodeId);

            if (node != null)
            {
                NodeService.RebootNode(node.Id);
            }

            return node;
        }

		/// <summary>
		/// POST /nodes
		/// </summary>
/*		public object Post(NodeBase p_node)
		{
			Db.Insert(p_node);

			var newNodeId = Db.GetLastInsertId();

			var newNode = new NodeResponse {
				Node = Db.Id<NodeBase>(newNodeId),
			};	
			return new HttpResult(newNode) {
				StatusCode = HttpStatusCode.Created,
				Headers = {
					{ HttpHeaders.Location, base.Request.AbsoluteUri.CombineWith(newNodeId.ToString()) }
				}
			};
		}*/

		/// <summary>
		/// PUT /nodes/{id}
		/// </summary>
	/*	public object Put(NodeBase p_node)
		{
			Db.Update(p_node);

			return new HttpResult {
				StatusCode = HttpStatusCode.NoContent,
				Headers = {
					{ HttpHeaders.Location, this.RequestContext.AbsoluteUri.CombineWith(p_node.Id.ToString()) }
				}
			};
		}*/

		/// <summary>
		/// DELETE /nodes/{Id}
		/// </summary>
	/*	public object Delete(NodeBase p_node)
		{
			Db.DeleteById<NodeBase>(p_node.Id);

			return new HttpResult {
				StatusCode = HttpStatusCode.NoContent,
				Headers = {
					{ HttpHeaders.Location, this.RequestContext.AbsoluteUri.CombineWith(p_node.Id.ToString()) }
				}
			};
		}*/
	}
}

