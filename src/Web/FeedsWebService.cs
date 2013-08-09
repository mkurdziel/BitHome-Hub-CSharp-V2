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
		public long? FeedId {get; set;}
	}

	[Csv(CsvBehavior.FirstEnumerable)]
	public class WebFeedsResponse
	{
		public long? FeedId {get; set;}
	}

	[ClientCanSwapTemplates]
	[DefaultView("Feeds")]
	public class FeedsWebService : Service
	{
		public object Get(WebFeeds request)
		{
			return new WebFeedsResponse {FeedId = request.FeedId};
		}
	}
}

