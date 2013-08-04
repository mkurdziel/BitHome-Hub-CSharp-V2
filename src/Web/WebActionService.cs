using System.Collections.Generic;
using ServiceStack.ServiceHost;
using BitHome.Actions;
using System;

namespace BitHome.Web
{
	[Route("/api/actions", "GET")]
	public class WebActions : IReturn<IAction[]> { }

	[Route("/api/actions/parameters", "GET")]
	public class WebActionParameters : IReturn<IParameter[]> { }

	[Route("/api/actions/node", "GET")]
	public class WebNodeActions : IReturn<WebNodeActionsResponse[]> { }

	[Route("/api/actions/{ActionId}/execute", "POST")]
	public class WebActionExecute : IReturn<IAction> {
        public string ActionId { get; set; }
        public Dictionary<string, string> parameters { get; set; }
    }

	[Route("/api/actions/{ActionId}/parameters", "GET")]
    public class WebActionParametersActionId : IReturn<IActionParameter[]> {
        public string ActionId { get; set; }
    }

	[Serializable]
	public class WebNodeActionsResponse {
		public Node Node { get; set; }
		public INodeAction[] Actions { get; set; }
	}

	public class WebActionService : ServiceStack.ServiceInterface.Service
	{
		
		public IAction[] Get(WebActions request) 
		{
			return ServiceManager.ActionService.Actions;
		}

		public IActionParameter[] Get(WebActionParameters request) 
		{
			return ServiceManager.ActionService.Parameters;
		}


		public IActionParameter[] Get(WebActionParametersActionId request)
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

        public IAction[] Post(WebActionExecute request)
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

		public WebNodeActionsResponse[] Get(WebNodeActions request) {
			List<WebNodeActionsResponse> response = new List<WebNodeActionsResponse> ();
			foreach (Node node in ServiceManager.NodeService.Nodes) {
				WebNodeActionsResponse resp = new WebNodeActionsResponse {
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

