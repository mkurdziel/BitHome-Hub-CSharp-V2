using System;
using BitHome.Actions;
using System.Collections.Generic;

namespace BitHome.Dashboards
{
	[Serializable]
	public class DashboardItem
	{
		public String Id { get; private set; }
		public String ActionId { get; set; }
		public long FeedId { get; set; }
		public String Name { get; set; }
		public String PositionX { get; set; }
		public String PositionY { get; set; }
		public bool ShowExecuteButton { get; set; }
		public Dictionary<String, DashboardItemValue> Values { get; set; }

		public DashboardItem() {
			Values = new Dictionary<string, DashboardItemValue> ();
		}

		public DashboardItem (String id, IAction action) : this()
		{
			this.Id = id;
			this.ActionId = action.Id;
			PositionX = "0";
			PositionY = "0";
			ShowExecuteButton = true;

			foreach (String parameterId in action.ParameterIds) {
				// TODO: fix this
				IParameter param = ServiceManager.ActionService.GetParameter (parameterId);

				if (param != null) {
					Values.Add (parameterId, new DashboardItemValue { 
						ParameterId = parameterId,
						Name = param.Name
					});
				}
			}
		}

        public DashboardItem(String id, Feeds.Feed feed) : this()
        {
			this.Id = id;
            this.FeedId = feed.Id;
        }
	}
}

