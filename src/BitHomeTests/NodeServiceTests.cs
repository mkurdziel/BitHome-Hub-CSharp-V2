using System;
using NUnit.Framework;
using BitHome;
using BitHome.Messaging;
using BitHome.Messaging.Messages;
using System.Threading;
using System.Collections.Generic;
using BitHome.Actions;
using BitHome.Messaging.Protocol;

namespace BitHomeTests
{
	[TestFixture()]
	public class NodeServiceTests
	{
		public static Dictionary<string,INodeAction> m_testActions = new Dictionary<string, INodeAction>();
		public static Dictionary<string, INodeParameter> m_testParams = new Dictionary<string, INodeParameter>();

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

		[Test()]
		public void TestNodeDiscovery () {

			TestNode node = GenerateTestNode ();

			PerformInvestigation (node);

			TestNode newNode = (TestNode)ServiceManager.NodeService.GetNode (node.Id);

			// TODO Name doesn't come from discovery, need to set
			node.Name = newNode.Name;

			ValidateNode (node, newNode);
		}

		[Test()]
		public void TestNodePersistence () {

			TestNode node = GenerateTestNode ();

			PerformInvestigation (node);

			TestNode newNode = (TestNode)ServiceManager.NodeService.GetNode (node.Id);

			// TODO Name doesn't come from discovery, need to set
			node.Name = newNode.Name;

			ValidateNode (node, newNode);

			ServiceManager.NodeService.WaitFinishSaving ();

			ServiceManager.RestartServices ();

			TestNode persistedNode = (TestNode)ServiceManager.NodeService.GetNode (newNode.Id);

			ValidateNode (node, persistedNode);
		}


		public static void PerformInvestigation(Node baseNode) {
			TestNode testNode = new TestNode { Id = baseNode.Id };

			MessageBase msg;

			// Wait for the inital message to be sent
			Thread.Sleep (TimeSpan.FromSeconds (1));

			ServiceManager.MessageDispatcherService.ClearNextMessageOut ();

			ServiceManager.NodeService.AddNode (testNode);

			// Wait for the node service to crank
			Thread.Sleep (TimeSpan.FromMilliseconds (10));

			// See if we've gotten an status investigation
			msg = ServiceManager.MessageDispatcherService.TakeNextMessageOut ();

			// Make sure that an investigation info request was sent out
			Assert.AreEqual (msg.Api , BitHome.Messaging.Protocol.Api.DEVICE_STATUS_REQUEST);
			Assert.AreEqual (msg.DestinationNode.Id , baseNode.Id);
			Assert.AreEqual (ServiceManager.MessageDispatcherService.MessageOutQueueCount , 0);

			// Return a device status response
			msg = new MessageDeviceStatusResponse (testNode, DeviceStatusValue.ACTIVE, 1, baseNode.Revision);
			ServiceManager.MessageDispatcherService.ReceiveMessage (msg);

			// Wait for the node service to crank
			Thread.Sleep (TimeSpan.FromMilliseconds (10));

			// See if we've gotten an info investigation
			msg = ServiceManager.MessageDispatcherService.TakeNextMessageOut ();
			Assert.NotNull (msg);

			// Make sure that an investigation info request was sent out
			Assert.AreEqual (BitHome.Messaging.Protocol.Api.DEVICE_INFO_REQUEST, msg.Api);
			Assert.AreEqual (baseNode.Id, msg.DestinationNode.Id);
			Assert.AreEqual (0, ServiceManager.MessageDispatcherService.MessageOutQueueCount);

			// Return a device status response
			msg = new MessageDeviceInfoResponse (
				testNode, 
				baseNode.ManufacturerId, 
				(byte)baseNode.TotalNumberOfActions, 
				0, null);

			ServiceManager.MessageDispatcherService.ReceiveMessage (msg);

			// Wait a sec for the message to be propagated
			Thread.Sleep (TimeSpan.FromMilliseconds (10));

			// Check that we are investigating
			Assert.IsTrue (ServiceManager.NodeService.IsInvestigating);

			for (int actionNum = 0; actionNum < baseNode.Actions.Count; actionNum++) {
				INodeAction action = m_testActions[baseNode.GetActionId (actionNum)];

				// Check for a catalog request 1
				msg = ServiceManager.MessageDispatcherService.TakeNextMessageOut ();
				Assert.NotNull (msg);
				Assert.AreEqual (msg.Api , BitHome.Messaging.Protocol.Api.CATALOG_REQUEST);
				Assert.AreEqual (((MessageCatalogRequest)msg).ActionIndex , action.ActionIndex);
				Assert.AreEqual (msg.DestinationNode.Id , baseNode.Id);
				Assert.AreEqual (ServiceManager.MessageDispatcherService.MessageOutQueueCount , 0);


				// Respond with catalog 
				msg = new MessageCatalogResponse (
					testNode, action.ActionIndex, action.TotalParameterCount, action.ReturnDataType, action.Name);

				ServiceManager.MessageDispatcherService.ReceiveMessage (msg);
				
				// Wait a sec for the message to be propagated
				Thread.Sleep (TimeSpan.FromMilliseconds (10));
			}

			for (int actionNum = 0; actionNum < baseNode.Actions.Count; actionNum++) {
				INodeAction action = m_testActions[baseNode.GetActionId (actionNum)];


				for (int paramNum = 0; paramNum < action.ParameterCount; paramNum++) {
					INodeParameter param = m_testParams[action.GetParameterId (paramNum)];

					msg = ServiceManager.MessageDispatcherService.TakeNextMessageOut ();
					Assert.AreEqual (msg.Api , BitHome.Messaging.Protocol.Api.PARAMETER_REQUEST);
					Assert.AreEqual (action.ActionIndex, ((MessageParameterRequest)msg).ActionIndex);
					Assert.AreEqual (param.ParameterIndex, ((MessageParameterRequest)msg).ParameterIndex);
					Assert.AreEqual (msg.DestinationNode.Id , baseNode.Id);
					Assert.AreEqual (ServiceManager.MessageDispatcherService.MessageOutQueueCount , 0);

					// Respond with parameter 1
					msg = new MessageParameterResponse (
						testNode,
						action.ActionIndex,
						param.ParameterIndex,
						param.Name,
						param.DataType,
						param.MinimumValue,
						param.MaximumValue, 
						null);

					ServiceManager.MessageDispatcherService.ReceiveMessage (msg);

					// Wait a sec for the message to be propagated
					Thread.Sleep (TimeSpan.FromMilliseconds (10));
				}
			}

			Thread.Sleep (TimeSpan.FromMilliseconds (10));
		}

	
		[Test()]
		public void TestInvestigation()
		{
			MessageBase msg;

			// Check that we are not investigating
			Assert.IsFalse (ServiceManager.NodeService.IsInvestigating);

			msg = ServiceManager.MessageDispatcherService.TakeNextMessageOut ();

			// Make sure that an initial status request was sent out
			Assert.AreEqual (msg.Api, BitHome.Messaging.Protocol.Api.DEVICE_STATUS_REQUEST);
			Assert.AreEqual (ServiceManager.MessageDispatcherService.MessageOutQueueCount , 0);

			// Create a test node
			Node testNode = new TestNode();
			ServiceManager.NodeService.AddNode (testNode);

			// Wait for the node service to pick up the new node
			Thread.Sleep (TimeSpan.FromMilliseconds (10));

			// See if we've gotten an status investigation
			msg = ServiceManager.MessageDispatcherService.TakeNextMessageOut ();

			// Make sure that an investigation info request was sent out
			Assert.AreEqual (msg.Api , BitHome.Messaging.Protocol.Api.DEVICE_STATUS_REQUEST);
			Assert.AreEqual (msg.DestinationNode.Id , testNode.Id);
			Assert.AreEqual (ServiceManager.MessageDispatcherService.MessageOutQueueCount , 0);

			// Return a device status response
			msg = new MessageDeviceStatusResponse (testNode, DeviceStatusValue.ACTIVE, 1, testNode.Revision);
			ServiceManager.MessageDispatcherService.ReceiveMessage (msg);

			// Wait for the node service to crank
			Thread.Sleep (TimeSpan.FromMilliseconds (10));

			// See if we've gotten an info investigation
			msg = ServiceManager.MessageDispatcherService.TakeNextMessageOut ();

			// Make sure that an investigation info request was sent out
			Assert.AreEqual (BitHome.Messaging.Protocol.Api.DEVICE_INFO_REQUEST, msg.Api);
			Assert.AreEqual (testNode.Id, msg.DestinationNode.Id);
			Assert.AreEqual (0, ServiceManager.MessageDispatcherService.MessageOutQueueCount);

			// Return a device status response
			msg = new MessageDeviceInfoResponse (
				testNode, 
				testNode.ManufacturerId, 
				(byte)testNode.TotalNumberOfActions, 
				0, null);

			ServiceManager.MessageDispatcherService.ReceiveMessage (msg);

			// Wait a sec for the message to be propagated
			Thread.Sleep (TimeSpan.FromMilliseconds (10));

			// Check that we are investigating
			Assert.IsTrue (ServiceManager.NodeService.IsInvestigating);

			// Check for a catalog response
			msg = ServiceManager.MessageDispatcherService.TakeNextMessageOut ();
			Assert.AreEqual (msg.Api , BitHome.Messaging.Protocol.Api.CATALOG_REQUEST);
			Assert.AreEqual (((MessageCatalogRequest)msg).ActionIndex , 0);
			Assert.AreEqual (msg.DestinationNode.Id , testNode.Id);
			Assert.AreEqual (ServiceManager.MessageDispatcherService.MessageOutQueueCount , 0);

//			// Respond with the default catalog
//			msg = new MessageCatalogResponse (
//				testNode,
//				2,
//				0,
//				0,
//				BitHome.Messaging.Protocol.DataType.VOID,
//				null,
//				null);
//			ServiceManager.MessageDispatcherService.ReceiveMessage (msg);
//
//			// Wait a sec for the message to be propagated
//			Thread.Sleep (TimeSpan.FromMilliseconds (10));
//
//			// Check for a catalog request 1
//			msg = ServiceManager.MessageDispatcherService.TakeNextMessageOut ();
//			Assert.AreEqual (msg.Api , BitHome.Messaging.Protocol.Api.CATALOG_REQUEST);
//			Assert.AreEqual (((MessageCatalogRequest)msg).FunctionNum , 1);
//			Assert.AreEqual (msg.DestinationNode.Id , testNode.Id);
//			Assert.AreEqual (ServiceManager.MessageDispatcherService.MessageOutQueueCount , 0);
//
//			// Respond with catalog 1
//			msg = new MessageCatalogResponse (
//				testNode,
//				2,
//				1,
//				1,
//				BitHome.Messaging.Protocol.DataType.VOID,
//				new Dictionary<int, BitHome.Messaging.Protocol.DataType>(){{1,BitHome.Messaging.Protocol.DataType.WORD}},
//			"Function 1");
//			ServiceManager.MessageDispatcherService.ReceiveMessage (msg);
//
//			// Check for a catalog request 2
//			msg = ServiceManager.MessageDispatcherService.TakeNextMessageOut ();
//			Assert.AreEqual (msg.Api , BitHome.Messaging.Protocol.Api.CATALOG_REQUEST);
//			Assert.AreEqual (((MessageCatalogRequest)msg).FunctionNum , 2);
//			Assert.AreEqual (msg.DestinationNode.Id , testNode.Id);
//			Assert.AreEqual (ServiceManager.MessageDispatcherService.MessageOutQueueCount , 0);
//
//			// Respond with catalog 2
//			msg = new MessageCatalogResponse (
//				testNode,
//				2,
//				2,
//				2,
//				BitHome.Messaging.Protocol.DataType.BYTE,
//				new Dictionary<int, BitHome.Messaging.Protocol.DataType>(){
//				{1,BitHome.Messaging.Protocol.DataType.WORD},
//				{2,BitHome.Messaging.Protocol.DataType.DWORD}
//			},
//			"Function 1");
//			ServiceManager.MessageDispatcherService.ReceiveMessage (msg);
//
//
//			// Wait a sec for the message to be propagated
//			Thread.Sleep (TimeSpan.FromMilliseconds (10));
//
//			// Check for a parameter request 1,1
//			msg = ServiceManager.MessageDispatcherService.TakeNextMessageOut ();
//			Assert.AreEqual (msg.Api , BitHome.Messaging.Protocol.Api.PARAMETER_REQUEST);
//			Assert.AreEqual (1, ((MessageParameterRequest)msg).ActionIndex);
//			Assert.AreEqual (1, ((MessageParameterRequest)msg).ParameterIndex);
//			Assert.AreEqual (msg.DestinationNode.Id , testNode.Id);
//			Assert.AreEqual (ServiceManager.MessageDispatcherService.MessageOutQueueCount , 0);
//
//			// Respond with parameter 1
//			msg = new MessageParameterResponse (
//				testNode,
//				1,
//				1,
//				"action 1 param 1",
//				BitHome.Messaging.Protocol.DataType.WORD,
//				BitHome.Messaging.Protocol.ParamValidationType.UNSIGNED_RANGE,
//				0,
//				2000, 
//				null);
//
//			ServiceManager.MessageDispatcherService.ReceiveMessage (msg);
//
//			// Wait a sec for the message to be propagated
//			Thread.Sleep (TimeSpan.FromMilliseconds (10));
//
//			// Check for a parameter request 2,1
//			msg = ServiceManager.MessageDispatcherService.TakeNextMessageOut ();
//			Assert.AreEqual (msg.Api , BitHome.Messaging.Protocol.Api.PARAMETER_REQUEST);
//			Assert.AreEqual (((MessageParameterRequest)msg).ActionIndex, 2);
//			Assert.AreEqual (((MessageParameterRequest)msg).ParameterIndex, 1);
//			Assert.AreEqual (msg.DestinationNode.Id , testNode.Id);
//			Assert.AreEqual (ServiceManager.MessageDispatcherService.MessageOutQueueCount , 0);
//
//			// Respond with parameter 1
//			msg = new MessageParameterResponse (
//				testNode,
//				2,
//				1,
//				"action 2 param 1",
//				BitHome.Messaging.Protocol.DataType.WORD,
//				BitHome.Messaging.Protocol.ParamValidationType.UNSIGNED_RANGE,
//				0,
//				2000, 
//				null);
//
//			ServiceManager.MessageDispatcherService.ReceiveMessage (msg);
//
//			// Wait a sec for the message to be propagated
//			Thread.Sleep (TimeSpan.FromMilliseconds (10));
//
//			// Check for a parameter request 2,2
//			msg = ServiceManager.MessageDispatcherService.TakeNextMessageOut ();
//			Assert.AreEqual (msg.Api , BitHome.Messaging.Protocol.Api.PARAMETER_REQUEST);
//			Assert.AreEqual (((MessageParameterRequest)msg).ActionIndex, 2);
//			Assert.AreEqual (((MessageParameterRequest)msg).ParameterIndex, 2);
//			Assert.AreEqual (msg.DestinationNode.Id , testNode.Id);
//			Assert.AreEqual (ServiceManager.MessageDispatcherService.MessageOutQueueCount , 0);
//
//			// Respond with parameter 2
//			msg = new MessageParameterResponse (
//				testNode,
//				2,
//				2,
//				"action 2 param 2",
//				BitHome.Messaging.Protocol.DataType.DWORD,
//				BitHome.Messaging.Protocol.ParamValidationType.SIGNED_RANGE,
//				10,
//				4000, 
//				null);
//
//			ServiceManager.MessageDispatcherService.ReceiveMessage (msg);
//
//			// Wait a sec for the investigation to complete
//			Thread.Sleep (TimeSpan.FromSeconds (1));
//
//			Assert.AreEqual (NodeInvestigationStatus.Completed, testNode.InvestigationStatus);
//
//			// Check that we are not investigating
//			Assert.IsFalse (ServiceManager.NodeService.IsInvestigating);
		}

	
	public static void ValidateNode(TestNode baseNode, TestNode testNode) {
		Assert.AreEqual(baseNode, testNode);

		for (int actionIndex = 0; actionIndex < baseNode.TotalNumberOfActions; actionIndex++) {
			String baseActionId = baseNode.GetActionId (actionIndex);
			String testActionId = testNode.GetActionId (actionIndex);

			INodeAction baseAction = m_testActions [baseActionId];
			INodeAction testAction = (INodeAction)ServiceManager.ActionService.GetAction (testActionId);

			// Make sure they are equal  (IDs will be different)
			Assert.IsTrue (baseAction.EqualsExceptId (testAction));

			for (int parameterIndex = 0; parameterIndex < baseAction.ParameterCount; parameterIndex++) {
				String baseParamId = ((NodeAction)baseAction).GetParameterId(parameterIndex);
				String testParamId = ((NodeAction)testAction).GetParameterId(parameterIndex);

                Assert.NotNull(baseParamId);
                Assert.NotNull(testParamId);

				INodeParameter baseParam = m_testParams [baseParamId];
				INodeParameter testParam = (INodeParameter)ServiceManager.ActionService.GetParameter (testParamId);

				Assert.IsTrue (baseParam.EqualsExceptId (testParam));
			}
		}
	}

	public static TestNode GenerateTestNode() {
		Random random = new Random();

		TestNode node = new TestNode ();
		node.Id = StorageService.GenerateKey ();

		// Random name
		node.Name = StorageService.GenerateKey();

		node.Revision = new BitHome.Version ();
		node.Revision.MajorVersion = (byte)random.Next (1,100);
		node.Revision.MinorVersion = (byte)random.Next (1,100);
			node.ManufacturerId = (UInt16)random.Next (1, 1000);

		// Generate the actions
		int actionCount = random.Next (1, 20);

		node.TotalNumberOfActions = actionCount;

		for (int i=0; i<actionCount; i++) {
			INodeAction action = GenerateTestNodeAction (node.Id, i);

			node.SetNodeAction (i, action.Id);

			m_testActions.Add (action.Id, action);
		}

		return node;
	}

	public static INodeAction GenerateTestNodeAction(string nodeId,int actionIndex) {
		Random random = new Random ();

		DataType returnType = (DataType)random.Next (1, 6);
		int paramCount = random.Next (0, 20);

		INodeAction action = new NodeAction (
			StorageService.GenerateKey (),
			nodeId,
			actionIndex,
			StorageService.GenerateKey (),
			returnType,
			paramCount);

		for (int i=0; i<paramCount; i++) {
			INodeParameter param = GenerateTestNodeParameter (nodeId, actionIndex, i, action.Id);

			action.AddNodeParameter (param);

			m_testParams.Add (param.Id, param);
		}

		return action;
	}

	public static INodeParameter GenerateTestNodeParameter(string nodeId,int actionIndex, int paramIndex, String actionId) {
		Random random = new Random ();

		DataType dataType = (DataType)random.Next (1, 6);
		int paramCount = random.Next (0, 20);

		INodeParameter param = new NodeParameter(
			nodeId,
			actionIndex,
			paramIndex,
			StorageService.GenerateKey(),
			StorageService.GenerateKey(),
			dataType,
			random.Next (0,100),
			random.Next (200,2000),
			null,
			actionId);

		return param;
	}
	}
}

