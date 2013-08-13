using System.Collections.Generic;
using BitHome;
using BitHome.Feeds;
using BitHome.Helpers;
using NLog;
using ServiceStack.ServiceHost;
using ServiceStack.Common.Web;
using System;
using System.Net;

namespace BitHome.WebApi
{
	[Route("/api/feeds", "GET")]
	public class WebApiFeeds : IReturn<Feed[]> {}

	[Route("/api/feeds", "POST")]
	public class WebApiFeedCreate : IReturn<Feed> {
        public String Name { get; set; }
    }

	[Route("/api/feeds/{feedId}", "GET")]
	public class WebApiFeed : IReturn<Feed> {
		public long FeedId { get; set; }
	}

	[Route("/api/feeds/{feedId}", "PUT")]
	public class WebApiFeedUpdate : IReturn<Feed> {
		public long FeedId { get; set; }
        public Dictionary<string, string> DataStreams { get; set; }
    }

	[Route("/api/feeds/{feedId}", "DELETE")]
	public class WebApiFeedDelete : IReturn<Feed> {
		public long FeedId { get; set; }
	}

	[Route("/api/feeds/{feedId}/datastreams", "GET")]
	public class WebApiDataStreams : IReturn<DataStream[]> {
		public long FeedId { get; set; }
	}

	[Route("/api/feeds/{feedId}/datastreams", "POST")]
	public class WebApiDataStreamsCreate : IReturn<DataStream> {
		public long FeedId { get; set; }
		public string Id { get; set; }
	}

	[Route("/api/feeds/{feedId}/datastreams/{dataStreamId}", "GET")]
	public class WebApiDataStream : IReturn<DataStream> {
		public long FeedId { get; set; }
		public string DataStreamId { get; set; }
	}

	[Route("/api/feeds/{feedId}/datastreams/{dataStreamId}", "PUT")]
	public class WebApiDataStreamUpdate : IReturn<DataStream> {
		public long FeedId { get; set; }
		public string DataStreamId { get; set; }
		public string Value { get; set; }
		public string MinValue { get; set; }
		public string MaxValue { get; set; }
	}

	[Route("/api/feeds/{feedId}/datastreams/{dataStreamId}", "DELETE")]
	public class WebApiDataStreamDelete : IReturn<DataStream> {
		public long FeedId { get; set; }
		public string DataStreamId { get; set; }
	}
	
	public class FeedsWebApiService : ServiceStack.ServiceInterface.Service
	{
        private static Logger log = LogManager.GetCurrentClassLogger();

		#region Feed Methods

		// Get all of the feeds
		public Feed[] Get(WebApiFeeds request) 
		{
			return ServiceManager.FeedService.Feeds;
		}

		// Post create feed
		public object Post(WebApiFeedCreate request) 
		{
			Feed feed = ServiceManager.FeedService.CreateFeed (request.Name);
			if (feed != null) {
				return feed;
			}
			return WebHelpers.BadRequestResponse;
		}

		// Get a single feed based on its ID
		public object Get(WebApiFeed request) 
		{
			Feed feed = ServiceManager.FeedService.GetFeed(request.FeedId); 

			if (feed != null) {
				return new WebFeed(feed);
			}
			return WebHelpers.NotFoundResponse;
		}

		// Update a feed based on its id
		public object Put(WebApiFeedUpdate request) 
		{
			Feed feed = ServiceManager.FeedService.GetFeed(request.FeedId); 

			if (feed != null) {
			    foreach (KeyValuePair<string, string> pair in request.DataStreams)
			    {
			        DataStream dataStream = feed.GetDataStream(pair.Key);
                    if (dataStream != null) {
                        ServiceManager.FeedService.SetDataStreamValue(feed, dataStream, pair.Value);
                    } else {
                        log.Warn("PUT FEED - trying to set value of null datastream");
                    }
			    }
				return feed;
			}
            return WebHelpers.NotFoundResponse;
        }

		// Delete a feed based on its id
		public object Delete(WebApiFeedUpdate request) 
		{
			Feed feed = ServiceManager.FeedService.GetFeed(request.FeedId); 

			if (feed != null) {
				if (ServiceManager.FeedService.DeleteFeed (feed.Id)) {
					return WebHelpers.OkResponse;
				}
			}
			return WebHelpers.NotFoundResponse;
		}


		#endregion

		#region DataStream Methods

		// Get all of the datastreams for a feed
		public object Get(WebApiDataStreams request) 
		{
			Feed feed = ServiceManager.FeedService.GetFeed(request.FeedId); 

			if (feed != null) {
				return feed.GetDataStreams();
			}
			return WebHelpers.NotFoundResponse;
		}

		// Create a datastream for a feed
		public object Post(WebApiDataStreamsCreate request) 
		{
			Feed feed = ServiceManager.FeedService.GetFeed(request.FeedId); 

			if (feed != null) {
				DataStream dataStream = ServiceManager.FeedService.CreateDataStream (feed.Id, request.Id);
				if (dataStream != null) {
					return dataStream;
				}
			}
			return WebHelpers.NotFoundResponse;
		}

		// Get a single datastream based on its ID
		public object Get(WebApiDataStream request) 
		{
			Feed feed = ServiceManager.FeedService.GetFeed(request.FeedId); 

			if (feed != null) {
				DataStream dataStream = feed.GetDataStream (request.DataStreamId);
				if (dataStream != null) {
					return dataStream;
				}
			}
			return WebHelpers.NotFoundResponse;
		}

		// Delete a datastream for a feed
		public object Put(WebApiDataStreamUpdate request) 
		{
			Feed feed = ServiceManager.FeedService.GetFeed(request.FeedId); 

			if (feed != null) {
				DataStream dataStream = feed.GetDataStream (request.DataStreamId);
				if (dataStream != null) {
					if (request.Value != null) {
						ServiceManager.FeedService.SetDataStreamValue (feed, dataStream, request.Value); 
					}
                    if (request.MaxValue != null) {
						ServiceManager.FeedService.SetMaxValue (feed, dataStream, request.MaxValue); 
                    }
                    if (request.MinValue != null) {
						ServiceManager.FeedService.SetMinValue (feed, dataStream, request.MinValue); 
                    }

				    return dataStream;
				}
			}
			return WebHelpers.NotFoundResponse;
		}	

		// Delete a datastream for a feed
		public object Delete(WebApiDataStreamDelete request) 
		{
			Feed feed = ServiceManager.FeedService.GetFeed(request.FeedId); 

			if (feed != null) {
				DataStream dataStream = feed.GetDataStream (request.DataStreamId);
				if (dataStream != null) {
					ServiceManager.FeedService.DeleteDataStream (feed, dataStream); 

					return dataStream;
				}
			}
			return WebHelpers.NotFoundResponse;
		}


		#endregion

	}
}

