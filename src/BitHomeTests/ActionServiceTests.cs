using System;
using NUnit.Framework;
using ServiceStack.Text;
using BitHome;
using BitHome.Actions;

namespace BitHomeTests
{

	[TestFixture()]
	public class ActionServiceTests
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

		[Test()]
		public void TestSaveActionList () {
			TestAction action1 = new TestAction();
			TestAction action2 = new TestAction();

			ServiceManager.ActionService.AddAction(action1);
			ServiceManager.ActionService.AddAction(action2);

			ServiceManager.ActionService.WaitFinishSaving ();
			ServiceManager.RestartServices ();

			String[] actionIds = ServiceManager.ActionService.ActionIds;

			Assert.Contains (action1.Id, actionIds);
			Assert.Contains (action1.Id, actionIds);

			TestAction action3 = (TestAction)ServiceManager.ActionService.GetAction(action1.Id);

			Assert.AreEqual(action1, action3);

			TestAction action4 = (TestAction)ServiceManager.ActionService.GetAction(action2.Id);

			Assert.AreEqual(action2, action4);
		}

        [Test()]
        public void TestSequenceAction()
        {
            // Create a test node with actions
            TestNode exampleNode = NodeServiceTests.GenerateTestNode();

            NodeServiceTests.PerformInvestigation(exampleNode);

            // Get the node into the system
            TestNode node = (TestNode) ServiceManager.NodeService.GetNode(exampleNode.Id);

            SequenceAction sequenceAction = new SequenceAction();

            foreach (String actionId in node.Actions.Values)
            {
                IAction nodeAction = ServiceManager.ActionService.GetAction(actionId);

                sequenceAction.AddAction(nodeAction);
            }

            Assert.AreEqual(node.TotalNumberOfActions, sequenceAction.ActionItems.Length);
        }
	}
}

