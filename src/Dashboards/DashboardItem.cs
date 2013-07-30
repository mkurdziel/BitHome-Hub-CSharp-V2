using System;
using BitHome.Actions;

namespace BitHome.Dashboards
{
	public class DashboardItem
	{
		public String Id { get; private set; }
		public IAction Action { get; set; }
		public String PositionX { get; set; }
		public String PositionY { get; set; }

		public DashboardItem (String id, IAction action)
		{
			this.Id = id;
			this.Action = action;
			PositionX = "0";
			PositionY = "0";
		}
	}
}

