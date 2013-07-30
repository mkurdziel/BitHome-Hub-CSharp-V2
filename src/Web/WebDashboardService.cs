using System.Collections.Generic;
using ServiceStack.ServiceHost;
using BitHome.Actions;
using BitHome.Dashboards;

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

	public class WebDashboardService : ServiceStack.ServiceInterface.Service
	{

		public Dashboard[] Get(WebDashBoards request) 
		{
			return ServiceManager.DashboardService.Dashboards;
		}

		public DashboardItem Post(WebDashboardItemName request)
		{
			return ServiceManager.DashboardService.SetItemPosition (
				request.DashboardItemId, 
				request.PositionX, 
				request.PositionY);
		}
	}
}

