﻿using System;
using Funq;
using ServiceStack.Common.Utils;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.Sqlite;
using ServiceStack.ServiceInterface.Cors;
using ServiceStack.Text;
using ServiceStack.WebHost.Endpoints;

namespace BitHome
{
	/// <summary>
	/// Create your ServiceStack web service application with a singleton AppHost.
	/// </summary>  
	public class BitHomeAppHost : AppHostBase
	{
		/// <summary>
		/// Initializes a new instance of your ServiceStack application, with the specified name and assembly containing the services.
		/// </summary>
		public BitHomeAppHost() : base("BitHome App Host", typeof(BitHomeService).Assembly) { }

		/// <summary>
		/// Configure the container with the necessary routes for your ServiceStack application.
		/// </summary>
		/// <param name="container">The built-in IoC used with ServiceStack.</param>
		public override void Configure(Container container)
		{
			//JsConfig.DateHandler = JsonDateHandler.ISO8601;

//			container.Register<IDbConnectionFactory>(c =>
//			                                         new OrmLiteConnectionFactory(
//				"~/App_Data/db.sqlite".MapHostAbsolutePath(),
//				SqliteOrmLiteDialectProvider.Instance));

			//Call existing service
//			using (var resetMovies = container.Resolve<ResetMoviesService>())
//			{
//				resetMovies.Any(null);
//			}

//			Routes
//				.Add<Movie>("/movies")
//					.Add<Movie>("/movies/{Id}")
//					.Add<Movies>("/movies")
//					.Add<Movies>("/movies/genres/{Genre}");

			Plugins.Add(new CorsFeature()); //Enable CORS

			SetConfig(new EndpointHostConfig {
				DebugMode = true, //Show StackTraces for easier debugging (default auto inferred by Debug/Release builds)
			});
		}
	}

	public class Global : System.Web.HttpApplication
	{
		protected void Application_Start(object sender, EventArgs e)
		{
			//Initialize your application
			(new BitHomeAppHost()).Init();
		}
	}
}
