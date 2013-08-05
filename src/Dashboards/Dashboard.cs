using System;
using System.Collections.Generic;
using ServiceStack.Text;
using BitHome.Actions;
using NLog;

namespace BitHome.Dashboards
{
	[Serializable]
	public class Dashboard
	{
		private static Logger log = LogManager.GetCurrentClassLogger();

		private Dictionary<String, DashboardItem> m_items;

		public String Id { get; private set; }

		public string Name { get; set; }

		public String[] DashboardItemIds {
			get {
				return m_items.Keys.ToArray ();
			}
		}

		public DashboardItem[] GetItems() {
			return m_items.Values.ToArray ();
		}

		public Dashboard (String id)
		{
			Id = id;
			m_items = new Dictionary<string, DashboardItem> ();
		}

		public void AddItem(DashboardItem item) {
			if (m_items.ContainsKey (item.Id)) {
				if (m_items[item.Id] == null) {
					m_items [item.Id] = item;
				} else {
					log.Warn("Replacing dashboard item {0} for dashboard {1}", item.Id, Id);
					m_items [item.Id] = item;
				}
			} else {
				m_items.Add (item.Id, item);
			}
		}

		public bool RemoveItem (string dashboardItemId)
		{
			if (m_items.ContainsKey(dashboardItemId)) {
				m_items.Remove(dashboardItemId);
				return true;
			}
			return false;
		}
	}
}

