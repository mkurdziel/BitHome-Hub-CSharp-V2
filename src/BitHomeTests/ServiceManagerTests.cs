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
	}
}

