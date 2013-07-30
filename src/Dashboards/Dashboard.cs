using System;
using System.Collections.Generic;
using ServiceStack.Text;
using BitHome.Actions;

namespace BitHome.Dashboards
{
	[Serializable]
	public class Dashboard
	{
		private Dictionary<String, DashboardItem> m_items;

		public String Id { get; private set; }

		public DashboardItem[] Items { 
			get {
				return m_items.Values.ToArray ();
			}
		}

		public Dashboard (String id)
		{
			Id = id;
			m_items = new Dictionary<string, DashboardItem> ();
		}

		public void AddActionItem (IAction action)
		{
			DashboardItem item = 
				ServiceManager.DashboardService.CreateDashboardItem (action);

			m_items.Add (item.Id, item);
		}
	}
}

