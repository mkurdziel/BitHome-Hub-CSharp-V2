using ServiceStack.ServiceHost;

namespace BitHome.Web
{
	[Route("/api/nodes", "GET")]
	public class WebNodes : IReturn<Node[]> { }

	[Route("/api/nodes/{NodeId}/name", "POST")]
	public class WebNodeName : IReturn<Node> {
        public string NodeId { get; set; }
        public string Name { get; set; }
    }

    [Route("/api/nodes/{NodeId}/reboot", "POST")]
    public class WebNodeReboot : IReturn<Node>
    {
        public string NodeId { get; set; }
    }

	public class NodeWebService : ServiceStack.ServiceInterface.Service
	{
		public NodeService NodeService {
			get {
				return ServiceManager.NodeService;
			}
		}

        public Node[] Get(WebNodes request) 
		{
			return NodeService.GetNodes ();
		}

        public Node Post(WebNodeName request)
        {
            Node node = NodeService.GetNode(request.NodeId);

            if (node!=null)
            {
                NodeService.SetNodeName(node.Id, request.Name);
            }

            return node;
        }


        public Node Post(WebNodeReboot request)
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

