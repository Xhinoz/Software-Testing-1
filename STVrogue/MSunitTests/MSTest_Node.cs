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

            player.AddNextCommand(0);
            node.fight(player);
            Assert.AreNotEqual(player.location, node);
            Assert.AreEqual(player.location, node2);
        }

        [TestMethod]
        public void MSTest_fight_use_potion()
        {
            Node node = new Node();
            Player player = new Player();
            Pack pack = new Pack("Ironman", 4);
            pack.location = node;
            node.packs.Add(pack);
            player.location = node;

            Item potion = new HealingPotion("potion");
            player.bag.Add(potion);
            player.AddNextCommand(1, 0);
            //node.fight(player);
            //Assert.IsFalse(player.bag.Contains(potion));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void MSTest_fight_fail_potion()
        {
            Node node = new Node();
            Player player = new Player();
            Pack pack = new Pack("Ok", 4);
            pack.location = node;
            node.packs.Add(pack);
            player.location = node;

            player.AddNextCommand(1, 0);
            node.fight(player);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void MSTest_fight_fail_crystal()
        {
            Node node = new Node();
            Player player = new Player();
            Pack pack = new Pack("Ok", 4);
            pack.location = node;
            node.packs.Add(pack);
            player.location = node;

            player.AddNextCommand(1, 1);
            node.fight(player);
        }

        [TestMethod]
        public void MSTest_fight_attack_quickend() // S6
        {
            Node node = new Node();
            Player player = new Player();
            Pack pack = new Pack("Goku", 1);
            node.packs.Add(pack);
            pack.location = node;
            player.location = node;


            Monster monster = pack.members[0];
            monster.HP = 1;
            player.AddNextCommand(2);
            node.fight(player);

            Assert.AreEqual(node.packs.Count, 0);
        }

        [TestMethod]
        public void MSTest_fight_Monsterturn_notfleeing() // S3 
        {
            Node node = new Node();
            Player player = new Player();
            Pack pack = new Pack("Goku", 1);
            node.packs.Add(pack);
            pack.location = node;
            player.location = node;

            Monster monster = pack.members[0];
            monster.HP = 6;
            player.AddNextCommand(2, 0, 2);
            node.fight(player);
            Assert.AreNotEqual(player.HP, player.HPbase);
        }

        [TestMethod]
        public void MSTest_fight_Monsterturn_fleeing() // S4
        {
            Node node = new Node();
            Node node2 = new Node();
            Player player = new Player();
            Dungeon dungeon = new Dungeon(1, 1);
            Pack pack = new Pack("Ren", 3);
            Pack pack2 = new Pack("Stimpy", 3);
            node.neighbors.Add(node2);
            node.packs.Add(pack);
            node.packs.Add(pack2);
            pack.dungeon = dungeon;
            pack2.dungeon = dungeon;
            pack.location = node;
            pack2.location = node;
            player.location = node;

            //player.AddNextCommand(2, 1);
            //node.fight(player);
            // One pack is at another node
            // One pack has attacked and damaged player
            //Assert.AreNotEqual(player.HP, player.HPbase); 
        }
    }
}
