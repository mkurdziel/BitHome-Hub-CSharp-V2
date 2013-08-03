using System;
using BitHome.Messaging.Protocol;
using NLog;
using ServiceStack.Text;
using System.Collections.Generic;
using System.Threading;

namespace BitHome.Actions
{
	public class ActionService : IBitHomeService
	{
		private const string KEY_ACTIONS = "actions";

		public const int ACTION_TIMEOUT_MS = 5000;
		public const int MAX_ACTION_THREADS = 10;
		public const int MAX_POOL_SIZE = 20;
		public const int CORE_POOL_SIZE = 10;
		public const int KEEP_ALIVE_TIME = 60;
		public const int THREAD_POOL_SIZE = 10;

		private static Logger log = LogManager.GetCurrentClassLogger();

		// List of all actions by Action ID
		// This allows fast lookup of Action when executing
		private Dictionary<string, IAction> m_actions = new Dictionary<string, IAction> ();

		// List of anonymous actions that were note associated with
		// a node but also are not a user-created action
		// These are created so there is only one instance of them in the system
		// and they can be used all over the place
//		private Dictionary<String, IAction> m_anonActionList = new Dictionary<string, IAction> ();

		// List of actions that were created by the user
		List<IAction> m_userActions = new List<IAction>();

		// Map of actions by node ID
		Dictionary<String, List<INodeAction>> m_nodeActions = new Dictionary<string, List<INodeAction>>();

		// Map of all parameters by parameter ID
		Dictionary<String, IParameter> m_parameters = new Dictionary<string, IParameter>();

		public IAction[] Actions {
			get {
				return m_actions.Values.ToArray ();
			}
		}

		public String[] ActionIds {
			get {
				return m_actions.Keys.ToArray ();
			}
		}

		#region Constructors

		public ActionService ()
		{
			ThreadPool.SetMaxThreads (THREAD_POOL_SIZE, 2);

			LoadData ();
		}

		#endregion Constructors

		private void LoadData() {
			// Load data from the storage service
			if (StorageService.Store<String[]>.Exists(KEY_ACTIONS)) {
				String[] actionKeys = StorageService.Store<String[]>.Get (KEY_ACTIONS);

				foreach (String key in actionKeys) 
				{
					if (StorageService.Store<IAction>.Exists (key)) {
						m_actions.Add (key, StorageService.Store<IAction>.Get (key));
					} else {
						m_actions.Add (key, new ActionUnknown (key));
					}
				}
			}
		}

		public bool Start()
		{
			log.Info ("Starting ActionService");

			return true;
		}

		public void Stop()
		{
			log.Info ("Stopping ActionService");

			// TODO stop all executing actions and wait
		}

		public INodeAction AddNodeAction (
			Node p_node, 
			int p_entryNumber, 
			string p_name, 
			DataType p_returnType, 
			int p_parameterCount)
		{
			log.Info ("Adding action to node:{0} index:{1} name:{2} returnType:{3} paramCount:{4}",
			         p_node.Id, p_entryNumber, p_name, p_returnType, p_parameterCount);

			// Get a unique key from the storage service
			String actionId = StorageService.GenerateKey ();	

			NodeAction action = new NodeAction (
				actionId,
				p_node.Id,
				p_entryNumber,
				p_name,
				p_returnType,
				p_parameterCount);

			AddAction (action);

			// Add it to the node map
			if (m_nodeActions.ContainsKey (p_node.Id)) {
				// TODO check for duplicates
				m_nodeActions [p_node.Id].Add (action);
			} else {
				m_nodeActions.Add (p_node.Id, new List<INodeAction> ());
			}

			// Set it to the node
			p_node.SetNodeAction (p_entryNumber, action.Id);

			return action;
		}

		public void AddAction ( IAction action) {

			if (action.Id == null) {
				// Get a unique key from the storage service
				String actionId = StorageService.GenerateKey ();	
				// Set that action id
				action.Id = actionId;
			}

			// Add the action to our internal map
			m_actions.Add (action.Id, action);
			// Save the action
			SaveAction (action);
			// Save the action list
			SaveActionList ();
		}

		public void AddNodeParameter (
			Node node, 
			int actionIndex, 
			int paramIndex, 
			string name, 
			DataType dataType, 
			ParamValidationType validationType, 
			long maxValue, 
			long minValue, 
			Dictionary<String, int> enumValues)
		{

			String actionId = node.GetActionId (actionIndex);

			if (actionId == null) {
				log.Warn ("Adding node parameter to null action id {0} of node {1}", actionIndex, node.Identifier);
				return;
			}

			INodeAction nodeAction = (INodeAction)GetAction (actionId);

			if (nodeAction == null) {
				log.Warn ("Adding node parameter to null action {0} of node {1}", actionIndex, node.Identifier);
				return;
			}

			NodeParameter nodeParam = new NodeParameter (
				node.Id,
				actionIndex,
				paramIndex,
				StorageService.GenerateKey (),
				name,
				dataType,
				validationType,
				minValue,
				maxValue,
				enumValues,
				nodeAction.Id);

			m_parameters.Add (nodeParam.Id, nodeParam);

			nodeAction.AddNodeParameter (nodeParam);
		}

		public IAction GetAction(String actionId) {
			if (m_actions.ContainsKey (actionId)) {
				return m_actions [actionId];
			}
			return null;
		}

		public IParameter GetParameter(String parameterId) {
			if (m_parameters.ContainsKey (parameterId)) {
				return m_parameters [parameterId];
			}
			return null;
		}

		public INodeAction[] GetNodeActions(String nodeId)
		{
			if (m_nodeActions.ContainsKey (nodeId)) {
				return m_nodeActions [nodeId].ToArray ();
			}
			return new INodeAction[]{};
		}

		public IAction[] GetUserActions()
		{
			return m_userActions.ToArray ();
		}

		public ActionRequest ExecuteAction(String actionId)
		{
			return ExecuteAction (actionId, ACTION_TIMEOUT_MS);
		}

		public ActionRequest ExecuteAction(String actionId, long timeout)
		{
			if (m_actions.ContainsKey (actionId)) {
				IAction action = m_actions [actionId];

				return ExecuteAction (action, timeout);
			}

			return null;
		}

		public ActionRequest ExecuteAction(IAction action, long timeout)
		{
			// TODO handle null action
			ActionRequest request = new ActionRequest (action, timeout);

			Execute (request);

			return request;
		}

		private void Execute(ActionRequest ActionRequest)
		{
			ThreadPool.QueueUserWorkItem (ActionRequest.ThreadPoolCallback);
		}

		#region Persistance Methods

		private void SaveActionList() 
		{
			StorageService.Store<String[]>.Insert (KEY_ACTIONS, this.ActionIds);
		}

		private void SaveAction(IAction action) 
		{
			StorageService.Store<IAction>.Insert (action.Id, action);
		}

		public void WaitFinishSaving ()
		{
			StorageService.Store<String[]>.WaitForCompletion ();
			StorageService.Store<IAction>.WaitForCompletion ();
		}
		#endregion

	}
}
