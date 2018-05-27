using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace STVRogue.GameLogic
{
    [TestClass]
    public class MSTest_Pack
    {
        [TestMethod]
        public void MSTest_pack_attack()
        {
            Pack pack = new Pack("Destroyer", 20);
            Player player = new Player();
            uint attr = pack.members[0].AttackRating;
            attr *= 20;
            pack.Attack(player);

            Assert.AreEqual(player.HP, (player.HPbase - attr));
        }

        [TestMethod]
        public void MSTest_pack_attack_stop()
        {
            Pack pack = new Pack("M'Baku", 110);
            Player player = new Player();
            pack.Attack(player);
            Assert.AreEqual(player.HP, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void MSTest_pack_moveException()
        {
            Pack pack = new Pack("Pack1", 4);
            Node node1 = new Node();
            Node node2 = new Node();

            pack.location = node1;
            pack.move(node2);
        }

        [TestMethod]
        public void MSTest_pack_move_fullcapacity()
        {
            Pack pack = new Pack("Hero", 3);
            Pack pack2 = new Pack("Gatekeeper", 3);
            Node node = new Node();
            Node node2 = new Node();
            Dungeon dungeon = new Dungeon(3, 3);
            pack.location = node;
            pack2.location = node2;
            pack.dungeon = dungeon;
            pack2.dungeon = dungeon;
            node.packs.Add(pack);
            node2.packs.Add(pack2);
            node.neighbors.Add(node2);

            pack.move(node2);
            Assert.AreNotEqual(pack.location, node2);
            Assert.AreEqual(pack.location, node);
        }
        [TestMethod]
        public void MSTest_pack_move_success()
        {
            Pack pack = new Pack("Mover", 3);
            Node node = new Node("node1");
            Node node2 = new Node("node2");
            Dungeon dungeon = new Dungeon(3, 3);
            pack.location = node;
            pack.dungeon = dungeon;
            node.packs.Add(pack);
            node.neighbors.Add(node2);

            pack.move(node2);
            Assert.AreNotEqual(pack.location, node);
            Assert.AreEqual(pack.location, node2);
        }
        [TestMethod]
        public void MSTest_pack_moveTowards()
        {
            Pack pack = new Pack("Highlander", 1);
            Dungeon dungeon = new Dungeon(2, 2);
            Node node = new Node("node1");
            Node node2 = new Node("node2");
            Node node3 = new Node("node3");
            Node node4 = new Node("node4");
            Node node5 = new Node("node5");
            Node node6 = new Node("node6");

            node.connect(node2);
            node2.connect(node3);
            node3.connect(node4);
            node.connect(node5);
            node5.connect(node2);
            node6.connect(node);
            pack.location = node;
            pack.dungeon = dungeon;
            node.packs.Add(pack);

            pack.moveTowards(node4);
            Assert.AreEqual(pack.location, node2);
        }
    }
}
