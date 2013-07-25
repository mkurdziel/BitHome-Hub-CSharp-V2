using System;
using NUnit.Framework;
using ServiceStack.Text;
using BitHome;
using BitHome.Actions;

namespace BitHomeTests
{
	public class TestAction : ActionBase {

		public bool Executed {get; set;}

		public TestAction() : base("testkey") {
			Executed = false;
		}
		#region implemented abstract members of ActionBase

		public override bool Execute (long timeout)
		{
			Executed = true;

			return true;
		}

		public override ActionType ActionType {
			get {
				return ActionType.Delay;
			}
		}

		#endregion


	}

	[TestFixture()]
	public class ActionServiceTest
	{
		[SetUp]
		public void SetUp()
		{
			ServiceManager.Start (true);
		}

		[TearDown]
		public void TearDown()
		{
			ServiceManager.Stop ();
		}

		[Test()]
		public void TestActionExecute ()
		{
			TestAction action = new TestAction ();
			ServiceManager.ActionService.AddAction (action);

			Assert.IsFalse (action.Executed);

			ActionRequest request = ServiceManager.ActionService.ExecuteAction (action.Id);

			request.WaitForExecution();

			Assert.IsTrue (action.Executed);
			Assert.AreEqual (ActionRequestStatus.Executed, request.Status);
		}
	}
}

