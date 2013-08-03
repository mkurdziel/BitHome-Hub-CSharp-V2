using System;
using NUnit.Framework;
using BitHome;

namespace BitHomeTests
{
	[TestFixture()]
	public class NodeServiceTests
	{
		[SetUp]
		public void SetUp () {
			ServiceManager.Start (true);
		}

		[TearDown]
		public void TearDown () {
			ServiceManager.Stop ();
		}

		[Test()]
		public void TestSaveNodeList () {
			TestNode node1 = new TestNode { };
			TestNode node2 = new TestNode { };

			ServiceManager.NodeService.AddNode (node1);
			ServiceManager.NodeService.AddNode (node2);

			ServiceManager.NodeService.WaitFinishSaving();

			ServiceManager.RestartServices ();

			String[] nodeIds = ServiceManager.NodeService.NodeIds;

			Assert.Contains (node1.Id, nodeIds);
			Assert.Contains (node2.Id, nodeIds);

			TestNode node3 = (TestNode)ServiceManager.NodeService.GetNode(node1.Id);

			Assert.AreEqual(node1, node3);

			TestNode node4 = (TestNode)ServiceManager.NodeService.GetNode(node2.Id);

			Assert.AreEqual(node2, node4);
		}
	}
}

