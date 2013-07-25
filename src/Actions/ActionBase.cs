using System;
using System.Collections.Generic;
using BitHome.Messaging.Protocol;
using NLog;
using System.Threading;

namespace BitHome.Actions
{
	[Serializable]
	public abstract class ActionBase : IAction
	{
		private const string DEFAULT_NAME = "Unknown Action";
		private static Logger log = LogManager.GetCurrentClassLogger();


		public String Id { get; set; }
		public String Name { get; set; }
		public String ExecutionErrorString { get; set; }
		public String Description { get; set; }
		public DataType ReturnDataType { get; set; }
		public String ReturnValue { get; set; }

		private List<IActionParameter> m_parameters = new List<IActionParameter>();
		private object m_actionLock = new object();

		public abstract ActionType ActionType { get; }

		IActionParameter[] IAction.Parameters {
			get {
				return m_parameters.ToArray ();
			}
		}

		IActionParameter[] IAction.InputParameters {
			get {
				List<IActionParameter> inputParameters = new List<IActionParameter> ();
				foreach (IActionParameter param in m_parameters) {
					if (param.ParameterType == ActionParameterType.Input) {
						inputParameters.Add (param);
					}
				}
				return inputParameters.ToArray();
			}
		}

		public int ParameterCount {
			get {
				return m_parameters.Count;
			}
		}
		
		public String Identifier {
			get {
				return Id;
			}
		}


		#region Constructors 

		public ActionBase (String id) {
			this.Id = id;
		}


		private ActionBase() {
			// Private constructor so we always insantiate with an id
		}

		#endregion

		#region Methods

		public void AddParameter(IActionParameter p_parameter)
		{
			log.Debug ("Adding parameter {0} to action {1}", p_parameter.Identifier, this.Identifier);

			m_parameters.Add (p_parameter);
		}

		public IActionParameter GetParameter(int p_index)
		{
			if (m_parameters.Count > p_index) {
				return m_parameters [p_index];
			} else {
				return null;
			}
		}

		public abstract bool Execute (long p_timeoutMilliseconds);


		#endregion


		public void RemoveAllParameters()
		{
			m_parameters.Clear ();
		}

		public void PrepareExecute()
		{
			ExecutionErrorString = String.Empty;

			Monitor.Enter (m_actionLock);
		}

		public void FinishExecute()
		{
			Monitor.Exit (m_actionLock);
		}
	}
}
