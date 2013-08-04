using System;
using BitHome.Dashboards;
using BitHome.Actions;

namespace BitHome
{
	[Serializable]
	public class WebDashboardItem
	{
		private DashboardItem m_dashboardItem;

		public IAction Action { get; private set; }
		public IActionParameter[] Parameters { get; private set; }

		public String Id { get { return m_dashboardItem.Id; } } 
		public String ActionId { get { return m_dashboardItem.ActionId; } }
		public String PositionX { get { return m_dashboardItem.PositionX; } }
		public String PositionY { get { return m_dashboardItem.PositionY; } }

		public WebDashboardItem (DashboardItem dashboardItem, 
		                         IAction dashboardAction, 
		                         IActionParameter[] dashboardParameters )
		{
			m_dashboardItem = dashboardItem;
			this.Action = dashboardAction;
			this.Parameters = dashboardParameters;
		}
	}
}

