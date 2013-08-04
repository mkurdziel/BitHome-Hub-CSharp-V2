using System.Collections.Generic;
using ServiceStack.ServiceHost;
using BitHome.Actions;
using BitHome.Dashboards;
using System;

namespace BitHome.Web
{
	[Route("/api/dashboards/{dashboardId}/items/{dashboardItemId}/position", "POST")]
	public class WebDashboardItemName : IReturn<DashboardItem> {
		public string DashboardId { get; set; }
		public string DashboardItemId { get; set; }
		public string PositionX { get; set; }
		public string PositionY { get; set; }
	}

	[Route("/api/dashboards", "GET")]
	public class WebDashBoards : IReturn<Dashboard[]> { }

	[Route("/api/dashboards", "POST")]
	public class WebDashBoardPost : IReturn<Dashboard> { 
		public string Name { get; set; }
		public string NodeId { get; set; }
	}

	[Route("/api/dashboards/{dashboardId}/delete", "POST")]
	public class WebDashBoardDelete : IReturn<bool> { 
		public string DashboardId { get; set; }
	}

	[Route("/api/dashboards/{dashboardId}/items", "GET")]
	public class WebDashBoardItems : IReturn<DashboardItem[]> {
		public string DashboardId { get; set; }
	}

	[Route("/api/dashboards/{dashboardId}/name", "POST")]
	public class WebDashBoardName : IReturn<bool> { 
		public string DashboardId { get; set; }
		public string Name { get; set; }
	}

	public class WebDashboardService : ServiceStack.ServiceInterface.Service
	{

		public Dashboard[] Get(WebDashBoards request) 
		{
			return ServiceManager.DashboardService.Dashboards;
		}

		public WebDashboardItem[] Get(WebDashBoardItems request) 
		{
			Dashboard dashboard = ServiceManager.DashboardService.GetDashboard (request.DashboardId);
			List<WebDashboardItem> webItems = new List<WebDashboardItem> ();

			if (dashboard != null) {
				foreach( DashboardItem item in	dashboard.GetItems () ) { 
					IAction action = ServiceManager.ActionService.GetAction (item.ActionId);
					// TODO optimize this
					List<IActionParameter> parameters = new List<IActionParameter> ();
					foreach (String parameterId in action.ParameterIds) {
						// TODO remove the cast
						parameters.Add ((IActionParameter)ServiceManager.ActionService.GetParameter (parameterId));
					}
					webItems.Add (new WebDashboardItem (item, action, parameters.ToArray()));
				}

				return webItems.ToArray ();
			}
			return null;
		}

		public DashboardItem Post(WebDashboardItemName request)
		{
			return ServiceManager.DashboardService.SetItemPosition (
				request.DashboardItemId, 
				request.PositionX, 
				request.PositionY);
		}

		public bool Post(WebDashBoardDelete request)
		{
			Dashboard dashboard = ServiceManager.DashboardService.GetDashboard (request.DashboardId);

			if (dashboard != null) {
				ServiceManager.DashboardService.RemoveDashboard (dashboard);
				return true;
			}
			return false;
		}

		
		public bool Post(WebDashBoardName request)
		{
			Dashboard dashboard = ServiceManager.DashboardService.GetDashboard (request.DashboardId);

			if (dashboard != null) {
				ServiceManager.DashboardService.SetDashboardName (dashboard, request.Name);
				return true;
			}
			return false;
		}

		public Dashboard Post(WebDashBoardPost request)
		{
			Dashboard dashboard;

			if (String.IsNullOrEmpty (request.NodeId) == false) {
				dashboard = ServiceManager.DashboardService.CreateDashboardFromNode (request.NodeId, request.Name);
			} else {
				dashboard = ServiceManager.DashboardService.CreateDashboard (request.Name);
			}

			return dashboard;
		}
	}
}

