using System;
using NUnit.Framework;
using BitHome;
using BitHome.Dashboards;
using BitHome.Actions;

namespace BitHomeTests
{
	[TestFixture()]
	public class DashboardServiceTests
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
		public void TestPersistence () {

			Dashboard db1 = ServiceManager.DashboardService.CreateDashboard ();

			ServiceManager.DashboardService.WaitFinishSaving ();

			ServiceManager.RestartServices ();

			Dashboard pdb1 = ServiceManager.DashboardService.GetDashboard (db1.Id);

			Assert.AreEqual (db1, pdb1);
		}
	}
}

