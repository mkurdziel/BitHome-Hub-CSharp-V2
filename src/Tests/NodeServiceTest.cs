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
        }

        [TearDown]
        public void CleanUp() {
        }


        [Test]
<<<<<<< HEAD
        public void TestAddNode() {
			Assert.IsTrue(true);
=======
        public void TestAddNode()
        {
            NodeBase node1 = new NodeBase();
            node1.Id = BinaryRage.Key.GenerateUniqueKey();
            node1.Name = "Node 1";


>>>>>>> 5971fb275e9571a61f792d0a9623cd99d61b65d3
        }
    }
}
