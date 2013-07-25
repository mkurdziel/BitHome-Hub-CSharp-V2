using System;
using System.Collections.Generic;
using System.Net;
using ServiceStack.Common;
using ServiceStack.Common.Web;
using ServiceStack.OrmLite;
using ServiceStack.ServiceHost;
using ServiceStack.Text;
using BitHome;
using BitHome.Actions;

namespace BitHome
{
	[Route("/actions", "GET")]
	public class WebActions : IReturn<IAction> { }

	public class WebActionService : ServiceStack.ServiceInterface.Service
	{
		
		public IAction[] Get(WebActions request) 
		{
			return ServiceManager.ActionService.Actions;
		}
	}
}

