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
		private static readonly Logger log = LogManager.GetCurrentClassLogger();

		private readonly object m_actionLock = new object();
		private List<String> m_parameterIds = new List<string>();

		public String Id { get; set; }
		public String Name { get; set; }
		public String ExecutionErrorString { get; set; }
		public String Description { get; set; }
		public DataType ReturnDataType { get; set; }
		public String ReturnValue { get; set; }
		public abstract ActionType ActionType { get; }

		public String Identifier {
			get {
				return Id;
			}
		}


		#region Constructors 

		public ActionBase (String id) {
			this.Id = id;
		}


		public ActionBase() {
			Id = null;
		}

		#endregion

		#region Methods

		#region Parameter Methods

		public virtual void AddParameter (IActionParameter parameter)
		{
			m_parameterIds.Add (parameter.Id);
		}

		public string[] ParameterIds {
			get {
				return m_parameterIds.ToArray ();
			}
		}

		public int ParameterCount {
			get {
				return m_parameterIds.Count;
			}
		}
		
		public void RemoveAllParameters()
		{
			m_parameterIds.Clear ();
		}

		#endregion

		#region Execution

		public void PrepareExecute()
		{
			ExecutionErrorString = String.Empty;

			Monitor.Enter (m_actionLock);
		}
	
		public abstract bool Execute (long p_timeoutMilliseconds);

		public void FinishExecute()
		{
			Monitor.Exit (m_actionLock);
		}

		#endregion Exection

		#endregion


		public bool EqualsExceptId (IAction obj)
		{
			// Return true if the fields match:
			bool same = true;

			same &= this.Name == obj.Name;
			same &= this.Description == obj.Description;
			same &= this.ReturnDataType == obj.ReturnDataType;
			same &= this.ActionType == obj.ActionType;

			return same;
		}

		public override bool Equals (object obj)
		{
			// If is null return false.
			if (obj == null)
			{
				return false;
			}

			// If cannot be cast to this class return false.
			ActionBase p = obj as ActionBase;
			if ((System.Object)p == null)
			{
				return false;
			}

			// Return true if the fields match:
			bool same = true;

			same &= this.Id == p.Id;
			same &= this.EqualsExceptId (p);

			return same;
		}

	}
}
