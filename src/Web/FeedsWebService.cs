using System.Collections.Generic;
using ServiceStack.OrmLite;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using ServiceStack.Text;
using ServiceStack.Common.Web;
using System;

namespace BitHome.Web
{
	[Route("/feeds")]
	[Route("/feeds/{feedId}")]
	public class WebFeeds
	{
		public int? FeedId { get; set; }
	}

	[Csv(CsvBehavior.FirstEnumerable)]
	public class WebFeedsResponse
	{
		public int? FeedId { get; set; }
	}

	[ClientCanSwapTemplates]
	[DefaultView("feedsView")]
	public class FeedsWebService : Service
	{
		public object Get(WebFeeds request)
		{
			return new WebFeedsResponse ();
		}
	}
}

