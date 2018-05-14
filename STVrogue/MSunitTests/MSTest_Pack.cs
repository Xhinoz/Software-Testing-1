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

            Assert.IsTrue(player.HP == (player.HPbase - attr));
        }

        [TestMethod]
        public void MSTest_pack_attack_stop()
        {
            Pack pack = new Pack("M'Baku", 110);
            Player player = new Player();
            pack.Attack(player);
            Assert.IsTrue(player.HP == 0);
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
            // Dungeon scenario full
            // Assert.IsFalse(move);
        }
        [TestMethod]
        public void MSTest_pack_move_success()
        {

        }
    }
}
