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
            Pack pack = new Pack("Ironman", 1);
            pack.location = node;
            node.packs.Add(pack);
            player.location = node;
            Monster monster = pack.members[0];
            monster.HP = 1;

            Item potion = new HealingPotion("potion");
            player.bag.Add(potion);
            player.AddNextCommand(1, 0, 2);
            node.fight(player);
            Assert.IsFalse(player.bag.Contains(potion));
        }
        [TestMethod]
        public void MSTest_fight_use_crystal()
        {

            Node node = new Node();
            Player player = new Player();
            Pack pack = new Pack("Warmachine", 1);
            pack.location = node;
            node.packs.Add(pack);
            player.location = node;
            Monster monster = pack.members[0];
            monster.HP = 1;

            Item crystal = new Crystal("crystal");
            player.bag.Add(crystal);
            player.AddNextCommand(1, 1, 2);
            node.fight(player);
            Assert.IsFalse(player.bag.Contains(crystal));
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
        public void MSTest_fight_Monsterturn_notfleeing() // S3 Attack
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
        public void MSTest_fight_Monterturn_fled() // S3 Flee
        {
            Node node = new Node();
            Node node2 = new Node();
            Player player = new Player();
            Dungeon dungeon = new Dungeon(3, 3);
            Pack pack = new Pack("Zordon", 2);
            node.neighbors.Add(node2);
            node.packs.Add(pack);
            pack.dungeon = dungeon;
            pack.location = node;
            player.location = node;

            player.AddNextCommand(2, 1);
            node.fight(player);
            Assert.AreEqual(pack.location, node2);
        }
        [TestMethod]
        public void MSTest_fight_Monsterturn_fled_success_multiplepacks() // S4
        {
            Node node = new Node();
            Node node2 = new Node();
            Player player = new Player();
            Dungeon dungeon = new Dungeon(1, 3);
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

            player.AddNextCommand(2, 1, 0);
            node.fight(player);
            // One pack is at another node
            // One pack has attacked and damaged player
            Assert.AreNotEqual(player.HP, player.HPbase); 
        }

        [TestMethod]
        public void MSTest_fight_Monsterturn_fled_unsuccessful() 
        {
            Node node = new Node();
            Node node2 = new Node();
            Player player = new Player();
            Dungeon dungeon = new Dungeon(3, 3);
            Pack pack = new Pack("Zordon", 2);
            Pack pack2 = new Pack("Alpha 5", 3);
            node.neighbors.Add(node2);
            node.packs.Add(pack);
            node2.packs.Add(pack2);
            pack.dungeon = dungeon;
            pack2.dungeon = dungeon;
            pack.location = node;
            pack2.location = node2;
            player.location = node;

            player.AddNextCommand(2, 1, 0);
            node.fight(player);
            Assert.AreEqual(pack.location, node);
            Assert.AreNotEqual(player.HP, player.HPbase);
        }
    }
}
