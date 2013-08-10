using System.Collections.Generic;
using BitHome.Feeds;
using BitHome.Helpers;
using ServiceStack.OrmLite;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using ServiceStack.Text;
using ServiceStack.Common.Web;
using System;

namespace BitHome.Web
{
	[Route("/feeds/{feedId}")]
	public class WebFeeds
	{
		public long? FeedId {get; set;}
	}

	[Csv(CsvBehavior.FirstEnumerable)]
	public class WebFeedsResponse
	{
		public Feed Feed {get; set;}
	}

	[ClientCanSwapTemplates]
	[DefaultView("Feed")]
	public class FeedsWebService : Service
	{
		public object Get(WebFeeds request)
		{
            if (request.FeedId != null)
            {
                Feed feed = ServiceManager.FeedService.GetFeed((long)request.FeedId);

                if (feed != null)
                {
                    return new WebFeedsResponse {Feed = feed};
                }

                return WebHelpers.NotFoundResponse;
            }

		    return WebHelpers.BadRequestResponse;
		}
	}
}

