using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using STVRogue.GameLogic;
using System.Collections.Generic;

namespace STVRogue.Utils
{
    [TestClass]
    public class MSTest_Predicates
    {
        [TestMethod]
        public void MSTest_reachableNodes()
        {
            Node N0 = new Node("N0");
            Node N1 = new Node("N1");
            Node N2 = new Node("N2");
            Node N3 = new Node("N3");
            Node N4 = new Node("N4");
            N0.connect(N1);
            N1.connect(N2);
            N1.connect(N3);
            N3.connect(N2);

            Predicates utils = new Predicates();
            List<Node> V = utils.reachableNodes(N0);
            foreach (Node O in V) Console.WriteLine("** " + O.id);
            Assert.IsTrue(V.Contains(N0));
            Assert.IsTrue(V.Contains(N1));
            Assert.IsTrue(V.Contains(N2));
            Assert.IsTrue(V.Contains(N3));
            Assert.IsFalse(V.Contains(N4));
        }
    }
}
