using System;
using System.Collections.Generic;
using System.Text;

namespace BitHome.Tests
{
    using NUnit.Framework;

    [TestFixture]
    public class NodeServiceTest
    {
        [SetUp]
        public void Init() {
			ServiceManager.Start (true);
        }

        [TearDown]
        public void CleanUp() {
        }


        [Test]
        public void TestAddNode()
        {
            Node node1 = new Node();
            node1.Id = BinaryRage.Key.GenerateUniqueKey();
            node1.Name = "Node 1";
        }
    }
}
