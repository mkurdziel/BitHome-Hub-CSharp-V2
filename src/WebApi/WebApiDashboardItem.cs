using System;
using BitHome.Dashboards;
using BitHome.Actions;
using BitHome.Feeds;

namespace BitHome.WebApi
{
	[Serializable]
	public class WebApiDashboardItem
	{
		private DashboardItem m_dashboardItem;

		public Feed Feed { get; private set; }
		public IAction Action { get; private set; }
		public IActionParameter[] Parameters { get; private set; }
		public DataStream[] DataStreams { get; private set; }

		public String Id { get { return m_dashboardItem.Id; } } 
		public String FeedId { get { 

            return m_dashboardItem.FeedId != null ? m_dashboardItem.FeedId.ToString() : null; 
        } }
		public String ActionId { get { return m_dashboardItem.ActionId; } }
		public String PositionX { get { return m_dashboardItem.PositionX; } }
		public String PositionY { get { return m_dashboardItem.PositionY; } }
		public String Name { get { return m_dashboardItem.Name; } }
		public bool ShowExecuteButton { get { return m_dashboardItem.ShowExecuteButton; } }

		public WebApiDashboardItem (DashboardItem dashboardItem, 
		                         IAction dashboardAction, 
		                         IActionParameter[] dashboardParameters )
		{
			m_dashboardItem = dashboardItem;
			this.Action = dashboardAction;
			this.Parameters = dashboardParameters;
		}

        public WebApiDashboardItem(DashboardItem dashboardItem,
                         Feed dashboardFeed,
                         DataStream[] dashboardDataStreams)
        {
            m_dashboardItem = dashboardItem;
            this.Feed = dashboardFeed;
            this.DataStreams = dashboardDataStreams;
        }
	}
}

