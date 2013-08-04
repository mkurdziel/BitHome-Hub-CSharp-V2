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
			m_items.Add (item.Id, item);
		}
	}
}

