using System;
using BitHome.Actions;

namespace BitHome.Dashboards
{
	[Serializable]
	public class DashboardItem
	{
		public String Id { get; private set; }
		public String ActionId { get; set; }
		public String PositionX { get; set; }
		public String PositionY { get; set; }

		public DashboardItem (String id, IAction action)
		{
			this.Id = id;
			this.ActionId = action.Id;
			PositionX = "0";
			PositionY = "0";
		}
	}
}

