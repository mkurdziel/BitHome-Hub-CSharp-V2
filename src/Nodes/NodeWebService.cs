using System.Collections.Generic;
using System.Net;
using ServiceStack.Common;
using ServiceStack.Common.Web;
using ServiceStack.OrmLite;
using ServiceStack.ServiceHost;
using ServiceStack.Text;

namespace BitHome
{
	public class NodeResponse
	{
		public NodeBase Node { get; set; }
	}

	/// <summary>
	/// Define your ServiceStack web service request (i.e. Request DTO).
	/// </summary>
	/// <remarks>The route is defined here rather than in the AppHost.</remarks>
	[Api("Find nodes by type, or all nodes if no genre is provided")]
	[Route("/nodes", "GET, OPTIONS")]
	[Route("/nodes/genres/{Genre}")]
	public class Nodes
	{
		public string Genre { get; set; }
	}

	public class NodesResponse
	{
		public List<NodeBase> Nodes { get; set; }
	}

	public class NodeWebService : ServiceStack.ServiceInterface.Service
	{
		public NodeService NodeService {
			get {
				return ServiceManager.NodeService;
			}
		}	


		public NodeWebService ()
		{
		}
/*
		/// <summary>
		/// GET /nodes/{Id} 
		/// </summary>
		public object Get(NodeBase p_node) {
			return new NodeResponse {
				Node = Db.Id<NodeBase>(p_node.Id),
			};	
		}*/

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

	public class NodesWebService : ServiceStack.ServiceInterface.Service {
		/// <summary>
		/// GET /movies 
		/// GET /movies/genres/{Genre}
		/// </summary>
	/*	public object Get(Nodes request)
		{
			return new NodesResponse {
				Nodes = request.Genre.IsNullOrEmpty()
					? Db.Select<NodeBase>()
						: Db.Select<NodeBase>("Genres LIKE {0}", "%{0}%".Fmt(request.Genre))
			};
		}*/
	}

}

