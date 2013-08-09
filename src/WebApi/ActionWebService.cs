using System.Collections.Generic;
using ServiceStack.ServiceHost;
using BitHome.Actions;
using System;

namespace BitHome.WebApi
{
	[Route("/api/actions", "GET")]
	public class WebApiActions : IReturn<IAction[]> { }

	[Route("/api/actions/parameters", "GET")]
	public class WebApiActionParameters : IReturn<IParameter[]> { }

	[Route("/api/actions/node", "GET")]
	public class WebApiNodeActions : IReturn<WebApiNodeActionsResponse[]> { }

	[Route("/api/actions/{ActionId}/execute", "POST")]
	public class WebApiActionExecute : IReturn<IAction> {
        public string ActionId { get; set; }
        public Dictionary<string, string> parameters { get; set; }
    }

	[Route("/api/actions/{ActionId}/parameters", "GET")]
	public class WebApiActionParametersActionId : IReturn<IActionParameter[]> {
        public string ActionId { get; set; }
    }

	[Serializable]
	public class WebApiNodeActionsResponse {
		public Node Node { get; set; }
		public INodeAction[] Actions { get; set; }
	}

	public class ActionWebApiService : ServiceStack.ServiceInterface.Service
	{
		
		public IAction[] Get(WebApiActions request) 
		{
			return ServiceManager.ActionService.Actions;
		}

		public IActionParameter[] Get(WebApiActionParameters request) 
		{
			return ServiceManager.ActionService.Parameters;
		}


		public IActionParameter[] Get(WebApiActionParametersActionId request)
        {
            IAction action = ServiceManager.ActionService.GetAction(request.ActionId);
            if (action != null)
            {
				List<IActionParameter> actionParms = new List<IActionParameter>(action.ParameterCount);

				// TODO check for nullsies
				foreach (string paramId in action.ParameterIds) {
					actionParms.Add ((IActionParameter)ServiceManager.ActionService.GetParameter (paramId));
				}

				return actionParms.ToArray ();
            }
            return new IActionParameter[0];
        }

		public IAction[] Post(WebApiActionExecute request)
        {
            foreach(string key in request.parameters.Keys)
            {
                IParameter param = ServiceManager.ActionService.GetParameter(key);
                if (param != null)
                {
                    param.SetValue(request.parameters[key]);
                }
            }
            ServiceManager.ActionService.ExecuteAction(request.ActionId);

            return null;
        }

		public WebApiNodeActionsResponse[] Get(WebApiNodeActions request) {
			List<WebApiNodeActionsResponse> response = new List<WebApiNodeActionsResponse> ();
			foreach (Node node in ServiceManager.NodeService.Nodes) {
				WebApiNodeActionsResponse resp = new WebApiNodeActionsResponse {
					Node = node
				};

				// TODO optimize this
				List<INodeAction> actions = new List<INodeAction>();

				foreach (String actionId in node.Actions.Values) {
					actions.Add( (INodeAction)ServiceManager.ActionService.GetAction(actionId));
				}

				resp.Actions = actions.ToArray ();

				response.Add (resp);
			}
			return response.ToArray();
		}
	}
}

