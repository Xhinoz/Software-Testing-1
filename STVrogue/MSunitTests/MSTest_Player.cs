using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace STVRogue.GameLogic
{
    /* An example of a test class written using VisualStudio's own testing
     * framework. 
     * This one is to unit-test the class Player. The test is incomplete though, 
     * as it only contains two test cases. 
     */
    [TestClass]
    public class MSTest_Player
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void MSTest_use_onEmptyBag()
        {
            Player P = new Player();
            P.use(new Item());
        }

        [TestMethod]
        public void MSTest_use_item_in_bag()
        {
            Player P = new Player();
            Item x = new HealingPotion("pot1");
            P.bag.Add(x);
            P.use(x);
            Assert.IsFalse(P.bag.Contains(x));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void MSTest_attackException()
        {
            Node n = new Node();
            Player player = new Player();
            Player player2 = new Player();
            player.Attack(player2);
        }

        [TestMethod]
        public void MSTest_player_normal_attack()
        {
            Node n = new Node();
            Player player = new Player();
            Pack pack = new Pack("Pack1", 5);

            Monster monster = pack.members[0];
            monster.HP = 6;
            player.Attack(monster);
            Assert.AreNotEqual(monster.HP, 6);
            player.Attack(monster); // Exception thrown when killed?
            Assert.AreEqual(pack.members.Count, 4);
            Assert.AreEqual(player.KillPoint, 1);
        }
        [TestMethod]
        public void MSTest_player_crystal_attack()
        {
            Player player = new Player();
            Pack pack = new Pack("Gang", 5);
            Item crystal = new Crystal("crystal");
            player.bag.Add(crystal);
            player.use(crystal);

            Monster monster = pack.members[0];
            Monster monster2 = pack.members[1];
            monster.HP = 6; // Attack Rating Player + 1
            monster2.HP = 5;
            player.Attack(monster);
            Assert.AreNotEqual(monster.HP, 6);
            Assert.AreNotEqual(pack.members.Count, 5);
        }
        // Item class
        [TestMethod]
        public void MSTest_use_potion()
        {
            Player player = new Player();
            Item potion = new HealingPotion("potion");
            player.bag.Add(potion);
            player.HP = 50;

            player.use(potion);
            Assert.AreNotEqual(player.HP, 50);
        }

        [TestMethod]
        public void MSTest_use_crystal()
        {
            Player player = new Player();
            Item crystal = new Crystal("crystal");
            player.bag.Add(crystal);
            player.use(crystal);
            Assert.IsTrue(player.accelerated);
        }

        [TestMethod]
        public void MSTest_used_item()
        {
            Player player = new Player();
            Item item = new Item();
            item.use(player);
            item.use(player);
            Assert.IsTrue(item.used);
        }
    }
}
