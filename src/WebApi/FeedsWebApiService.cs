using BitHome;
using BitHome.Feeds;
using BitHome.Helpers;
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
		public string DataStreamId { get; set; }
	}

	[Route("/api/feeds/{feedId}/datastream/{dataStreamId}", "GET")]
	public class WebApiDataStream : IReturn<DataStream> {
		public long FeedId { get; set; }
		public string DataStreamId { get; set; }
	}

	[Route("/api/feeds/{feedId}/datastream/{dataStreamId}", "PUT")]
	public class WebApiDataStreamUpdate : IReturn<DataStream> {
		public long FeedId { get; set; }
		public string DataStreamId { get; set; }
	}

	[Route("/api/feeds/{feedId}/datastream/{dataStreamId}", "DELETE")]
	public class WebApiDataStreamDelete : IReturn<DataStream> {
		public long FeedId { get; set; }
		public string DataStreamId { get; set; }
	}
	
	public class FeedsWebApiService : ServiceStack.ServiceInterface.Service
	{
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
				return feed;
			}
			return WebHelpers.NotFoundResponse;
		}

		// Update a feed based on its id
		public object Put(WebApiFeedUpdate request) 
		{
			throw new NotImplementedException ();
//			Feed feed = ServiceManager.FeedService.GetFeed(request.FeedId); 
//
//			if (feed != null) {
//				return feed;
//			}
//			return NotFoundResponse;
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
				return feed.DataStreams;
			}
			return WebHelpers.NotFoundResponse;
		}

		// Create a datastream for a feed
		public object Post(WebApiDataStreamsCreate request) 
		{
			Feed feed = ServiceManager.FeedService.GetFeed(request.FeedId); 

			if (feed != null) {
				DataStream dataStream = ServiceManager.FeedService.CreateDataStream (feed.Id, request.DataStreamId);
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
					throw new NotImplementedException ();
//					return dataStream;
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
					feed.DeleteDataStream (dataStream.Id);
					return dataStream;
				}
			}
			return WebHelpers.NotFoundResponse;
		}


		#endregion

	}
}

