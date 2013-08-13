using System;
using BitHome.Actions;

namespace BitHome.Dashboards
{
	[Serializable]
	public class DashboardItem
	{
		public String Id { get; private set; }
		public String ActionId { get; set; }
		public long FeedId { get; set; }
		public String PositionX { get; set; }
		public String PositionY { get; set; }
		public String Name { get; set; }

		public DashboardItem (String id, IAction action)
		{
			this.Id = id;
			this.ActionId = action.Id;
			PositionX = "0";
			PositionY = "0";
		}

        public DashboardItem(String id, Feeds.Feed feed)
        {
			this.Id = id;
            this.FeedId = feed.Id;
        }
	}
}

