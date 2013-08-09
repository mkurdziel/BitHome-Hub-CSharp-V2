using System;
using BitHome.Dashboards;
using BitHome.Actions;

namespace BitHome.WebApi
{
	[Serializable]
	public class WebApiDashboardItem
	{
		private DashboardItem m_dashboardItem;

		public IAction Action { get; private set; }
		public IActionParameter[] Parameters { get; private set; }

		public String Id { get { return m_dashboardItem.Id; } } 
		public String ActionId { get { return m_dashboardItem.ActionId; } }
		public String PositionX { get { return m_dashboardItem.PositionX; } }
		public String PositionY { get { return m_dashboardItem.PositionY; } }
		public String Name { get { return m_dashboardItem.Name; } }

		public WebApiDashboardItem (DashboardItem dashboardItem, 
		                         IAction dashboardAction, 
		                         IActionParameter[] dashboardParameters )
		{
			m_dashboardItem = dashboardItem;
			this.Action = dashboardAction;
			this.Parameters = dashboardParameters;
		}
	}
}

