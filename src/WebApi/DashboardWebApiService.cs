using System.Collections.Generic;
using BitHome.Feeds;
using NUnit.Framework;
using ServiceStack.ServiceHost;
using BitHome.Actions;
using BitHome.Dashboards;
using System;
using BitHome.Helpers;

namespace BitHome.WebApi
{
	[Route("/api/dashboards/{dashboardId}", "GET")]
	public class WebApiDashboard : IReturn<Dashboard> {
		public string DashboardId { get; set; }
	}

	[Route("/api/dashboards/{dashboardId}/items/{dashboardItemId}/position", "POST")]
	public class WebApiDashboardItemPosition : IReturn<DashboardItem> {
		public string DashboardId { get; set; }
		public string DashboardItemId { get; set; }
		public string PositionX { get; set; }
		public string PositionY { get; set; }
	}

	[Route("/api/dashboards/{dashboardId}/items/{dashboardItemId}/delete", "POST")]
	public class WebApiDashboardItemRemove : IReturn<bool> {
		public string DashboardId { get; set; }
		public string DashboardItemId { get; set; }
	}

	[Route("/api/dashboards/{dashboardId}/items/{dashboardItemId}/name", "POST")]
	public class WebApiDashBoardItemName : IReturn<WebApiDashboardItem> { 
		public string DashboardId { get; set; }
		public string DashboardItemId { get; set; }
		public string Name { get; set; }
	}

	[Route("/api/dashboards/{dashboardId}/items", "POST")]
	public class WebApiDashboardItemCreate : IReturn<DashboardItem> {
		public string DashboardId { get; set; }
		public string[] ActionIds { get; set; }
		public long[] FeedIds { get; set; }
	}

	[Route("/api/dashboards", "GET")]
	public class WebApiDashBoards : IReturn<Dashboard[]> { }

	[Route("/api/dashboards", "POST")]
	public class WebApiDashBoardPost : IReturn<Dashboard> { 
		public string Name { get; set; }
		public string NodeId { get; set; }
	}

	[Route("/api/dashboards/{dashboardId}/delete", "POST")]
	public class WebApiDashBoardDelete : IReturn<bool> { 
		public string DashboardId { get; set; }
	}

	[Route("/api/dashboards/{dashboardId}/items", "GET")]
	public class WebApiDashBoardItems : IReturn<DashboardItem[]> {
		public string DashboardId { get; set; }
	}

    [Route("/api/dashboards/{dashboardId}/values", "GET")]
    public class WebApiDashBoardValues : IReturn<WebDashboardValue[]> 
    {
        public string DashboardId { get; set; }
    }

	[Route("/api/dashboards/{dashboardId}/name", "POST")]
	public class WebApiDashBoardName : IReturn<bool> { 
		public string DashboardId { get; set; }
		public string Name { get; set; }
	}

	public class DashboardWebApiService : ServiceStack.ServiceInterface.Service
	{

		public Dashboard[] Get(WebApiDashBoards request) 
		{
			return ServiceManager.DashboardService.Dashboards;
		}

		public object Get(WebApiDashboard request) {
			Dashboard dashboard = ServiceManager.DashboardService.GetDashboard (request.DashboardId);
			if (dashboard != null) {
				return dashboard;
			}
			return null;
		}

		public WebApiDashboardItem[] Get(WebApiDashBoardItems request) 
		{
			Dashboard dashboard = ServiceManager.DashboardService.GetDashboard (request.DashboardId);
			List<WebApiDashboardItem> webItems = new List<WebApiDashboardItem> ();

			if (dashboard != null) {
				foreach( String dashboardItemid in	dashboard.DashboardItemIds ) { 

					DashboardItem item = ServiceManager.DashboardService.GetDashboardItem(dashboardItemid);

                    if (item.ActionId != null) {
                        IAction action = ServiceManager.ActionService.GetAction (item.ActionId);
                        // TODO optimize this
                        List<IActionParameter> parameters = new List<IActionParameter> ();
                        foreach (String parameterId in action.ParameterIds) {
                            // TODO remove the cast
                            parameters.Add ((IActionParameter)ServiceManager.ActionService.GetParameter (parameterId));
                        }
                        webItems.Add (new WebApiDashboardItem (item, action, parameters.ToArray()));
                    } 
                    else if (item.FeedId > 0)
                    {
                        Feed feed = ServiceManager.FeedService.GetFeed(item.FeedId);
                        DataStream[] dataStreams = feed.GetDataStreams();

                        webItems.Add(new WebApiDashboardItem(item, feed, dataStreams));
                    }
				}

				return webItems.ToArray ();
			}
			return null;
		}

        public WebDashboardValue[] Get(WebApiDashBoardValues request)
        {
            Dashboard dashboard = ServiceManager.DashboardService.GetDashboard(request.DashboardId);

            List<WebDashboardValue> values = new List<WebDashboardValue>();

            if (dashboard != null)
            {
				HashSet<INodeParameter> parameterRequests = new HashSet<INodeParameter> ();

                foreach (String dashboardItemid in dashboard.DashboardItemIds)
                {
                    DashboardItem item = ServiceManager.DashboardService.GetDashboardItem(dashboardItemid);

					if (item.FeedId > 0) {
						Feed feed = ServiceManager.FeedService.GetFeed (item.FeedId);
						DataStream[] dataStreams = feed.GetDataStreams ();

						foreach (DataStream datastream in dataStreams) {
							values.Add (new WebDashboardValue {
								FeedId = item.FeedId.ToString(), 
								DataStreamId = datastream.Id, 
								Value = datastream.Value
							});
						}
					} else if(item.ActionId != null) {
						IAction action = ServiceManager.ActionService.GetAction (item.ActionId);
						if (action != null) {
							// TODO: Optimize this
							foreach (String paramId in action.ParameterIds) {
								IParameter param = ServiceManager.ActionService.GetParameter (paramId);

								// Add it to the hash to initiate data requests
								if (param is INodeParameter) {
									parameterRequests.Add ((INodeParameter)param);
								}

								String value = param.Value;
								if (value != null) {
									values.Add (new WebDashboardValue 
									            { ActionId = action.Id, ParameterId = paramId, Value = value });
								}
							}
						}
					}
                }

				ServiceManager.ActionService.SendDataRequests(parameterRequests);

                return values.ToArray();
            }
            return null;
        }

		public DashboardItem Post(WebApiDashboardItemPosition request)
		{
			return ServiceManager.DashboardService.SetItemPosition (
				request.DashboardItemId, 
				request.PositionX, 
				request.PositionY);
		}

		public DashboardItem Post(WebApiDashBoardItemName request)
		{
			return ServiceManager.DashboardService.SetDashboardItemName (
				request.DashboardId, 
				request.DashboardItemId, 
				request.Name);
		}

		public bool Post(WebApiDashboardItemCreate request)
		{
			if (request.ActionIds != null) {
				foreach (String actionId in request.ActionIds) {
                    ServiceManager.DashboardService.CreateDashboardItemFromAction(
						request.DashboardId,
						actionId);
				}
			}
            if (request.FeedIds != null)
            {
                foreach (long feedId in request.FeedIds)
                {
                    ServiceManager.DashboardService.CreateDashboardItemFromFeed(
                        request.DashboardId,
                        feedId);
                }
            }
			return true;
		}

		public bool Post(WebApiDashboardItemRemove request)
		{
			return ServiceManager.DashboardService.RemoveDashboardItem (
				request.DashboardId, 
				request.DashboardItemId);
		}

		public bool Post(WebApiDashBoardDelete request)
		{
			Dashboard dashboard = ServiceManager.DashboardService.GetDashboard (request.DashboardId);

			if (dashboard != null) {
				ServiceManager.DashboardService.RemoveDashboard (dashboard);
				return true;
			}
			return false;
		}

		
		public bool Post(WebApiDashBoardName request)
		{
			Dashboard dashboard = ServiceManager.DashboardService.GetDashboard (request.DashboardId);

			if (dashboard != null) {
				ServiceManager.DashboardService.SetDashboardName (dashboard, request.Name);
				return true;
			}
			return false;
		}

		public Dashboard Post(WebApiDashBoardPost request)
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

