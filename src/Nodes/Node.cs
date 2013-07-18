using System;
using System.Collections.Generic;
using System.Net;
using ServiceStack.Common;
using ServiceStack.Common.Web;
using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;
using ServiceStack.ServiceHost;
using ServiceStack.Text;

namespace BitHome
{
	/// <summary>
	/// Define your ServiceStack web service request (i.e. Request DTO).
	/// </summary>
	/// <remarks>The route is defined here rather than in the AppHost.</remarks>
	[Api("GET or DELETE a single node by Id. Use POST to create a new node and PUT to update it")]
	[Route("/nodes", "POST,PUT,PATCH,DELETE")]
	[Route("/nodes/{Id}")]
    [Serializable]
	public class Node
	{
		public String Id { get; set; }
		public String Name { get; set; }
		public DateTime LastSeen { get; set; }
		public NodeInvestigationStatus InvestigationStatus { get; set; }
		public bool IsUknown { get; set; }
		public NodeType NodeType { get; set; }
		public String MetaDataKey { get; set; }

		public String IdString {
			get {
				return String.Format ("0x{0:X16}", Id);
			}
		}

		public NodeStatus Status {
			get {
				if (IsUknown) {
					return NodeStatus.Unknown;
				}

				// Get the current datetime
				TimeSpan activeTime = DateTime.Now - LastSeen;

				// If the active time is less than five minutes, we are active
				if (activeTime < TimeSpan.FromMinutes(5)) {
					return NodeStatus.Acive;
				}

				if (activeTime < TimeSpan.FromHours(1)) {
					return NodeStatus.Recent;
				}
				return NodeStatus.Dead;
			}
		}


		public Node ()
		{
		}
	}
}

