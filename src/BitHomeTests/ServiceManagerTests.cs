using System;
using BitHome;
using BitHome.Messaging;
using BitHome.Messaging.Messages;

using NUnit.Framework;
using System.Threading;
using System.Collections.Generic;

namespace BitHomeTests
{
	[TestFixture()]
	public class ServiceManagerTests
	{
		[Test()]
		public void TestStartStop()
		{
			Assert.IsFalse(ServiceManager.IsStarted);
			ServiceManager.Start (true);
			Assert.IsTrue (ServiceManager.IsStarted);
			ServiceManager.Stop ();
			Assert.IsFalse (ServiceManager.IsStarted);
		}

		[Test()]
		public void TestQueryInterval()
		{
			MessageBase msg;

			ServiceManager.Start (true);

			// Make sure that an initial info request was sent out
			msg = ServiceManager.MessageDispatcherService.TakeNextMessageOut ();
			Assert.AreEqual (msg.Api, BitHome.Messaging.Protocol.Api.DEVICE_STATUS_REQUEST);
			Assert.AreEqual (((MessageDeviceStatusRequest)msg).RequestType, BitHome.Messaging.Protocol.DeviceStatusRequest.INFO_REQUEST);
			Assert.AreEqual (ServiceManager.MessageDispatcherService.MessageOutQueueCount, 0);

			Thread.Sleep (ServiceManager.NodeService.GetQueryInterval ());

			// Make sure that another info request was sent out
			msg = ServiceManager.MessageDispatcherService.TakeNextMessageOut ();
			Assert.AreEqual (msg.Api, BitHome.Messaging.Protocol.Api.DEVICE_STATUS_REQUEST);
			Assert.AreEqual (((MessageDeviceStatusRequest)msg).RequestType, BitHome.Messaging.Protocol.DeviceStatusRequest.STATUS_REQUEST);
			Assert.AreEqual (ServiceManager.MessageDispatcherService.MessageOutQueueCount, 0);

			ServiceManager.Stop ();
		}

		[Test()]
		public void TestInvestigation()
		{
			MessageBase msg;

			ServiceManager.Start (true);

			msg = ServiceManager.MessageDispatcherService.TakeNextMessageOut ();

			// Make sure that an initial info request was sent out
			Assert.AreEqual (msg.Api, BitHome.Messaging.Protocol.Api.DEVICE_STATUS_REQUEST);
			Assert.AreEqual (((MessageDeviceStatusRequest)msg).RequestType , BitHome.Messaging.Protocol.DeviceStatusRequest.INFO_REQUEST);
			Assert.AreEqual (ServiceManager.MessageDispatcherService.MessageOutQueueCount , 0);

			// Create a test node
			Node testNode = new TestNode();
			ServiceManager.NodeService.AddNode (testNode);

			// See if we've gotten an info investigation
			msg = ServiceManager.MessageDispatcherService.TakeNextMessageOut ();

			// Make sure that an investigation info request was sent out
			Assert.AreEqual (msg.Api , BitHome.Messaging.Protocol.Api.DEVICE_STATUS_REQUEST);
			Assert.AreEqual (((MessageDeviceStatusRequest)msg).RequestType , BitHome.Messaging.Protocol.DeviceStatusRequest.INFO_REQUEST);
			Assert.AreEqual (msg.DestinationNode.Id , testNode.Id);
			Assert.AreEqual (ServiceManager.MessageDispatcherService.MessageOutQueueCount , 0);

			// Return a device status response
			msg = new MessageDeviceStatusResponse (
				testNode, 
				BitHome.Messaging.Protocol.DeviceStatusValue.INFO,
				1);
			ServiceManager.MessageDispatcherService.ReceiveMessage (msg);

			// Wait a sec for the message to be propagated
			Thread.Sleep (TimeSpan.FromMilliseconds (10));

			// Check for a catalog response
			msg = ServiceManager.MessageDispatcherService.TakeNextMessageOut ();
			Assert.AreEqual (msg.Api , BitHome.Messaging.Protocol.Api.CATALOG_REQUEST);
			Assert.AreEqual (((MessageCatalogRequest)msg).FunctionNum , 0);
			Assert.AreEqual (msg.DestinationNode.Id , testNode.Id);
			Assert.AreEqual (ServiceManager.MessageDispatcherService.MessageOutQueueCount , 0);

			// Respond with the default catalog
			msg = new MessageCatalogResponse (
				testNode,
				2,
				0,
				0,
				BitHome.Messaging.Protocol.DataType.VOID,
				null,
				null);
			ServiceManager.MessageDispatcherService.ReceiveMessage (msg);

			// Wait a sec for the message to be propagated
			Thread.Sleep (TimeSpan.FromMilliseconds (10));

			// Check for a catalog request 1
			msg = ServiceManager.MessageDispatcherService.TakeNextMessageOut ();
			Assert.AreEqual (msg.Api , BitHome.Messaging.Protocol.Api.CATALOG_REQUEST);
			Assert.AreEqual (((MessageCatalogRequest)msg).FunctionNum , 1);
			Assert.AreEqual (msg.DestinationNode.Id , testNode.Id);
			Assert.AreEqual (ServiceManager.MessageDispatcherService.MessageOutQueueCount , 0);

			// Respond with catalog 1
			msg = new MessageCatalogResponse (
				testNode,
				2,
				1,
				1,
				BitHome.Messaging.Protocol.DataType.VOID,
				new Dictionary<int, BitHome.Messaging.Protocol.DataType>(){{1,BitHome.Messaging.Protocol.DataType.WORD}},
				"Function 1");
			ServiceManager.MessageDispatcherService.ReceiveMessage (msg);

			// Check for a catalog request 2
			msg = ServiceManager.MessageDispatcherService.TakeNextMessageOut ();
			Assert.AreEqual (msg.Api , BitHome.Messaging.Protocol.Api.CATALOG_REQUEST);
			Assert.AreEqual (((MessageCatalogRequest)msg).FunctionNum , 2);
			Assert.AreEqual (msg.DestinationNode.Id , testNode.Id);
			Assert.AreEqual (ServiceManager.MessageDispatcherService.MessageOutQueueCount , 0);

			// Respond with catalog 2
			msg = new MessageCatalogResponse (
				testNode,
				2,
				2,
				2,
				BitHome.Messaging.Protocol.DataType.BYTE,
				new Dictionary<int, BitHome.Messaging.Protocol.DataType>(){
					{1,BitHome.Messaging.Protocol.DataType.WORD},
					{2,BitHome.Messaging.Protocol.DataType.DWORD}
				},
			"Function 1");
			ServiceManager.MessageDispatcherService.ReceiveMessage (msg);


			// Wait a sec for the message to be propagated
			Thread.Sleep (TimeSpan.FromMilliseconds (10));

			// Check for a parameter request 1,1
			msg = ServiceManager.MessageDispatcherService.TakeNextMessageOut ();
			Assert.AreEqual (msg.Api , BitHome.Messaging.Protocol.Api.PARAMETER_REQUEST);
			Assert.AreEqual (1, ((MessageParameterRequest)msg).ActionIndex);
			Assert.AreEqual (1, ((MessageParameterRequest)msg).ParameterIndex);
			Assert.AreEqual (msg.DestinationNode.Id , testNode.Id);
			Assert.AreEqual (ServiceManager.MessageDispatcherService.MessageOutQueueCount , 0);

			// Respond with parameter 1
			msg = new MessageParameterResponse (
				testNode,
				1,
				1,
				"action 1 param 1",
				BitHome.Messaging.Protocol.DataType.WORD,
				BitHome.Messaging.Protocol.ParamValidationType.UNSIGNED_RANGE,
				0,
				2000, 
				null);

			ServiceManager.MessageDispatcherService.ReceiveMessage (msg);

			// Wait a sec for the message to be propagated
			Thread.Sleep (TimeSpan.FromMilliseconds (10));

			// Check for a parameter request 2,1
			msg = ServiceManager.MessageDispatcherService.TakeNextMessageOut ();
			Assert.AreEqual (msg.Api , BitHome.Messaging.Protocol.Api.PARAMETER_REQUEST);
			Assert.AreEqual (((MessageParameterRequest)msg).ActionIndex, 2);
			Assert.AreEqual (((MessageParameterRequest)msg).ParameterIndex, 1);
			Assert.AreEqual (msg.DestinationNode.Id , testNode.Id);
			Assert.AreEqual (ServiceManager.MessageDispatcherService.MessageOutQueueCount , 0);

			// Respond with parameter 1
			msg = new MessageParameterResponse (
				testNode,
				2,
				1,
				"action 2 param 1",
				BitHome.Messaging.Protocol.DataType.WORD,
				BitHome.Messaging.Protocol.ParamValidationType.UNSIGNED_RANGE,
				0,
				2000, 
				null);

			ServiceManager.MessageDispatcherService.ReceiveMessage (msg);

			// Wait a sec for the message to be propagated
			Thread.Sleep (TimeSpan.FromMilliseconds (10));

			// Check for a parameter request 2,2
			msg = ServiceManager.MessageDispatcherService.TakeNextMessageOut ();
			Assert.AreEqual (msg.Api , BitHome.Messaging.Protocol.Api.PARAMETER_REQUEST);
			Assert.AreEqual (((MessageParameterRequest)msg).ActionIndex, 2);
			Assert.AreEqual (((MessageParameterRequest)msg).ParameterIndex, 2);
			Assert.AreEqual (msg.DestinationNode.Id , testNode.Id);
			Assert.AreEqual (ServiceManager.MessageDispatcherService.MessageOutQueueCount , 0);

			// Respond with parameter 2
			msg = new MessageParameterResponse (
				testNode,
				2,
				2,
				"action 2 param 2",
				BitHome.Messaging.Protocol.DataType.DWORD,
				BitHome.Messaging.Protocol.ParamValidationType.SIGNED_RANGE,
				10,
				4000, 
				null);

			ServiceManager.MessageDispatcherService.ReceiveMessage (msg);

			// Wait a sec for the message to be propagated
			Thread.Sleep (TimeSpan.FromMilliseconds (10));

			Assert.AreEqual (NodeInvestigationStatus.Completed, testNode.InvestigationStatus);

			ServiceManager.Stop ();
		}
	}
}

