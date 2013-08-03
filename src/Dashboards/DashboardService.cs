using System;
using System.Collections.Generic;
using BitHome.Actions;
using ServiceStack.Text;

namespace BitHome.Dashboards 
{
	public class DashboardService
	{
		Dictionary<string, Dashboard> m_dashboards = null;
		Dictionary<string, DashboardItem> m_dashboardItems;

		public Dashboard[] Dashboards { 
			get 
			{
				if (m_dashboards == null) {
					UpdateDashboards ();
				}
				return m_dashboards.Values.ToArray();
			} 
		}

		public DashboardService ()
		{
			m_dashboardItems = new Dictionary<string, DashboardItem> ();
		}

		private void UpdateDashboards()
		{
			m_dashboards = new Dictionary<string, Dashboard> ();

			foreach (Node node in ServiceManager.NodeService.GetNodes()) {
				Dashboard dash = new Dashboard (StorageService.GenerateKey());

				m_dashboards.Add (dash.Id, dash);

//				foreach (INodeAction action in node.Actions) {
//					dash.AddActionItem (action);
//				}
			}
		}

		public DashboardItem CreateDashboardItem(IAction action) {
			DashboardItem item = new DashboardItem (
				StorageService.GenerateKey(), 
				action);

			m_dashboardItems.Add (item.Id, item);

			return item;
		}

		public DashboardItem GetDashboardItem (String id)
		{
			if (m_dashboardItems.ContainsKey (id)) {
				return m_dashboardItems[id];
			}
			return null;
		}

		public DashboardItem SetItemPosition (
			string dashboardItemId, 
			string positionX, 
			string positionY)
		{
			DashboardItem item = GetDashboardItem (dashboardItemId);
			if (item != null) {
				item.PositionX = positionX;
				item.PositionY = positionY;
			}

			return item;
		}
	}
}

