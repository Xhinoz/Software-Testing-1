using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using STVRogue.GameLogic;
using STVRogue.Utils;


namespace UnitTests_STVRogue
{
    [TestClass]
    public class MSTest_Game
    {


        [TestMethod]
        public void MSTest_create_valid_game()
        {
            uint difficultyLevel = 1;
            uint nodeCapacityMultiplier = 1;
            uint numberOfMonsters = 1;

            Game game = new Game(difficultyLevel, nodeCapacityMultiplier, numberOfMonsters);
            Assert.IsTrue(game.validGame);
        }

        [TestMethod]
        public void MSTest_correct_difficulty_level()
        {
            Predicates p = new Predicates();
            uint difficultyLevel = 3;
            Game game = new Game();
            game.dungeon = new Dungeon(difficultyLevel, 1);
            Assert.AreEqual(p.countNumberOfBridges(game.dungeon.startNode, game.dungeon.exitNode), difficultyLevel);
        }

        [TestMethod]
        public void MSTest_correct_numberOfMonsters()
        {
            Predicates p = new Predicates();
            uint numberOfMonsters = 10;
            Game game = new Game(3, 1, numberOfMonsters);
            int countMonsters = 0;
            foreach(Node n in p.reachableNodes(game.dungeon.startNode))
            {
                foreach(Pack pack in n.packs)
                {
                    countMonsters += pack.members.Count;
                }
            }

            Assert.AreEqual(numberOfMonsters, countMonsters);
        }

        [TestMethod]
        public void MSTest_check_node_capacity()
        {
            Predicates p = new Predicates();
            uint difficultyLevel = 3;
            uint nodeCapacityMultiplier = 1;
            uint numberOfMonsters = 1;

            Game game = new Game(difficultyLevel, nodeCapacityMultiplier, numberOfMonsters);
            foreach (Node n in p.reachableNodes(game.dungeon.startNode))
            {
                Assert.IsTrue(n.packs.Count < nodeCapacityMultiplier * (p.countNumberOfBridges(game.dungeon.startNode, n) + 1));
            }
        }

        [TestMethod]
        public void MSTest_seed_monster_pack_single()
        {
            uint difficultyLevel = 1;
            uint numberOfMonsters = 1;
            Game game = new Game();
            game.dungeon = new Dungeon(difficultyLevel, 1);
            game.SeedMonsterPacks(difficultyLevel, numberOfMonsters);
            Assert.AreEqual(1, game.monsterPacks.Count);

        }

        [TestMethod]
        public void MSTest_seed_monster_pack_multiple()
        {
            RandomGenerator.initializeWithSeed(1);
            uint difficultyLevel = 1;
            uint numberOfMonsters = 10;

            Game game = new Game();
            game.dungeon = new Dungeon(difficultyLevel, 1);
            game.SeedMonsterPacks(difficultyLevel, numberOfMonsters);
            Assert.AreEqual(1, game.monsterPacks.Count);

        }

        //create random monsterpacks with fixed seed
        //4 5 2 3 is with given seed of 1 and upperlimits is 14
        //test is total packs is correct and creatures per pack
        [TestMethod]
        public void MSTest_Create_Random_Monster_Packs()
        {
            Game g = new Game();
            RandomGenerator.initializeWithSeed(1);
            //4 5 2 3
            g.monsterPacks = g.CreateMonsterPacks(1, 14, 1);
            Assert.AreEqual(4, g.monsterPacks.Count);
            Assert.AreEqual(4, g.monsterPacks[0].members.Count);
            Assert.AreEqual(5, g.monsterPacks[1].members.Count);
            Assert.AreEqual(2, g.monsterPacks[2].members.Count);
            Assert.AreEqual(3, g.monsterPacks[3].members.Count);
        }

        //create a single pack of 1 creature
        [TestMethod]
        public void MSTest_Create_Single_Monster_Pack()
        {
            Game g = new Game();
            g.monsterPacks = g.CreateMonsterPacks(1, 1, 1);
            Assert.AreEqual(1, g.monsterPacks.Count);
        }

        [TestMethod]
        public void MSTest_Seed_Items_Check()
        {
            RandomGenerator.initializeWithSeed(1); //items = 7 (3hp, 4 crystals) monsterpacks = 19;
            Game g = new Game(3, 3, 10);
            Assert.AreEqual(g.items.Count, 7);

        }

        //hp of the items+player must be smaller than 0.8 * all monsters
        [TestMethod]
        public void MSTest_Potion_Property_Check_False()
        {
            Game g = new Game();
            Pack p = new Pack("test", 1);
            g.player = new Player();
            p.members[0].totalHP = 1;
            p.members[0].HP = 1;

            Assert.IsFalse(g.PotionProperty());
        }
        //hp of the items+player must be smaller than 0.8 * all monsters
        [TestMethod]
        public void MSTest_Potion_Property_Check_True()
        {
            //RandomGenerator.initializeWithSeed(1);
            Game g = new Game();
            Pack p = new Pack("test", 1);
            g.player = new Player();
            g.player.HP = 10;
            p.members[0].totalHP = 20;
            p.members[0].HP = 20;
            g.items.Add(new HealingPotion("1"));
            g.items[0].HPvalue = 10;

            Assert.IsFalse(g.PotionProperty());
        }

        [TestMethod]
        public void MSTest_command_valid()
        {
            Game g = new Game();
            STVRogue.Command command = new STVRogue.Command();
            Assert.IsTrue(g.update(command));
        }

    }
}
