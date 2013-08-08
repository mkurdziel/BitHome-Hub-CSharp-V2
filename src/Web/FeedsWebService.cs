using BitHome;
using BitHome.Feeds;
using ServiceStack.ServiceHost;
using ServiceStack.Common.Web;
using System;
using System.Net;

namespace BitHome
{
	[Route("/api/feeds", "GET")]
	public class WebFeeds : IReturn<Feed[]> {}

	[Route("/api/feeds/{feedId}", "GET")]
	public class WebFeed : IReturn<Feed> {
		public string FeedId { get; set; }
	}

	[Route("/api/feeds/{feedId}/datastreams", "GET")]
	public class WebDataStreams : IReturn<DataStream[]> {
		public string FeedId { get; set; }
	}

	[Route("/api/feeds/{feedId}/datastreams", "POST")]
	public class WebDataStreamsCreate : IReturn<DataStream> {
		public string FeedId { get; set; }
		public string DataStreamId { get; set; }
	}

	[Route("/api/feeds/{feedId}/datastream/{dataStreamId}", "GET")]
	public class WebDataStream : IReturn<DataStream> {
		public string FeedId { get; set; }
		public long DataStreamId { get; set; }
	}

	[Route("/api/feeds/{feedId}/datastream/{dataStreamId}", "DELETE")]
	public class WebDataStreamDelete : IReturn<DataStream> {
		public string FeedId { get; set; }
		public long DataStreamId { get; set; }
	}
	
	public class FeedsWebService : ServiceStack.ServiceInterface.Service
	{
		#region Feed Methods

		// Get all of the feeds
		public Feed[] Get(WebFeeds request) 
		{
			return ServiceManager.FeedService.Feeds;
		}

		// Get a single feed based on its ID
		public object Get(WebFeed request) 
		{
			Feed feed = ServiceManager.FeedService.GetFeed(request.FeedId); 

			if (feed != null) {
				return feed;
			}
			return NotFoundResponse;
		}

		#endregion

		#region DataStream Methods

		// Get all of the datastreams for a feed
		public object Get(WebDataStreams request) 
		{
			Feed feed = ServiceManager.FeedService.GetFeed(request.FeedId); 

			if (feed != null) {
				return feed.DataStreams;
			}
			return NotFoundResponse;
		}

		// Get a single datastream based on its ID
		public object Get(WebDataStream request) 
		{
			Feed feed = ServiceManager.FeedService.GetFeed(request.FeedId); 

			if (feed != null) {
				DataStream dataStream = feed.GetDataStream (request.DataStreamId);
				if (dataStream != null) {
					return dataStream;
				}
			}
			return NotFoundResponse;
		}

		// Create a datastream for a feed
		public object Post(WebDataStreamsCreate request) 
		{
			Feed feed = ServiceManager.FeedService.GetFeed(request.FeedId); 

			if (feed != null) {
				DataStream dataStream = ServiceManager.FeedService.CreateDataStream (feed.Id, request.DataStreamId);
				if (dataStream != null) {
					return dataStream;
				}
			}
			return NotFoundResponse;
		}

		// Delete a datastream for a feed
		public object Delete(WebDataStreamDelete request) 
		{
			Feed feed = ServiceManager.FeedService.GetFeed(request.FeedId); 

			if (feed != null) {
				DataStream dataStream = feed.GetDataStream (request.DataStreamId);
				if (dataStream != null) {
					feed.DeleteDataStream (dataStream.Id);
					return dataStream;
				}
			}
			return NotFoundResponse;
		}


		#endregion

		#region Helper Methods

		private object NotFoundResponse {
			get {
				return new HttpResult() { StatusCode = HttpStatusCode.NotFound };
			}
		}

		#endregion
	}
}

