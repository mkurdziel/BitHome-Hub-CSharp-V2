using System.Collections.Generic;
using BitHome.Feeds;
using BitHome.Helpers;
using NLog;
using ServiceStack.OrmLite;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using ServiceStack.Text;
using ServiceStack.Common.Web;
using System;
using BitHome.Dashboards;
using System.Net;

namespace BitHome.Web
{
	[Route("/dashboards")]
	public class WebDashboards { }

	[Route("/dashboard")]
	public class WebDashboardRedirect { }

	[Route("/dashboards/{dashboardId}")]
	public class WebDashboard
	{
		public string DashboardId {get; set;}
	}

	[Csv(CsvBehavior.FirstEnumerable)]
	public class WebDashboardResponse
	{
		public Dashboard Dashboard {get; set;}
		public string DashboardId {get; set;}
	}

	[ClientCanSwapTemplates]
	[DefaultView("DashboardView")]
	public class DashboardsWebService : Service
	{
		public object Get(WebDashboards request)
		{
			return new HttpResult() {
				View = "Dashboards"
			};
		}
		public object Get(WebDashboardRedirect request) {
			base.Response.AddHeader("Location", "/dashboards");
			return new HttpResult() { StatusCode = HttpStatusCode.Redirect };
		}

		public object Get(WebDashboard request)
		{
			if (request.DashboardId != null)
			{
				Dashboard dashboard = ServiceManager.DashboardService.GetDashboard(request.DashboardId);

				if (dashboard != null)
				{
					return new WebDashboardResponse {Dashboard =  dashboard, DashboardId =  request.DashboardId};
				}

				return WebHelpers.NotFoundResponse;
			}

			return WebHelpers.BadRequestResponse;
		}
	}
}

