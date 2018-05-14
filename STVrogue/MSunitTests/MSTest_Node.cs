using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace STVRogue.GameLogic
{
    [TestClass]
    public class MSTest_Node
    {
        [TestMethod]
        public void MSTest_fight_flee()
        {
            Node node = new Node();
            Node node2 = new Node();
            Player player = new Player();
            Pack pack = new Pack("packo", 4);
            node.connect(node2);

            player.location = node;
            node.packs.Add(pack);
            pack.location = node;

            node.fight(player, 0);
            Assert.IsTrue(player.location != node);
            Assert.IsTrue(player.location == node2);
        }

        [TestMethod]
        public void MSTest_fight_use_item()
        {
            Node node = new Node();
            Player player = new Player();
            player.location = node;

            Item item = new Item();
            
        }
    }
}
