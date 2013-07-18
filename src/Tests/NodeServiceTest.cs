using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BitHome.Tests
{
    using NUnit.Framework;

    [TestFixture]
    public class NodeServiceTest
    {
        [SetUp]
        public void Init() {
        }

        [TearDown]
        public void CleanUp() {
        }


        [Test]
        public void TestAddNode()
        {
            NodeBase node1 = new NodeBase();
            node1.Id = BinaryRage.Key.GenerateUniqueKey();
            node1.Name = "Node 1";


        }
    }
}
