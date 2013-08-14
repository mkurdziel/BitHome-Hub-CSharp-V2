using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BitHome.WebApi
{
    [Serializable]
    public class WebDashboardValue
    {
        public String FeedId { get; set; }
        public String DataStreamId { get; set; }
        public String Value { get; set; }
    }
}
