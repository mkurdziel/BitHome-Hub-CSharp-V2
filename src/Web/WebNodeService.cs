using System.Collections.Generic;
using System.Net;
using ServiceStack.Common;
using ServiceStack.Common.Web;
using ServiceStack.OrmLite;
using ServiceStack.ServiceHost;
using ServiceStack.Text;
using BitHome;

namespace BitHome.Web
{
	[Route("/nodes", "GET")]
	public class AllNodes : IReturn<Node[]> { }

	public class NodeResponse
	{
		public Node Node { get; set; }
	}

	public class NodesResponse
	{
		public Node[] Nodes { get; set; }
	}

	public class WebNodeService : ServiceStack.ServiceInterface.Service
	{
		public NodeService NodeService {
			get {
				return ServiceManager.NodeService;
			}
		}	

		public Node[] Get(AllNodes request) 
		{
			return NodeService.GetNodes ();
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

