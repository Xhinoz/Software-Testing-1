﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STVRogue.Utils;

namespace STVRogue.GameLogic
{
    public class Game
    {
        public Player player;
        public Dungeon dungeon;
        public List<Item> items;
        public List<Pack> monsterPacks;
        private Predicates predicates;
        public bool validGame;

        /* This creates a player and a random dungeon of the given difficulty level and node-capacity
         * The player is positioned at the dungeon's starting-node.
         * The constructor also randomly seeds monster-packs and items into the dungeon. The total
         * number of monsters are as specified. Monster-packs should be seeded as such that
         * the nodes' capacity are not violated. Furthermore the seeding of the monsters
         * and items should meet the balance requirements stated in the Project Document.
         */
        public Game(uint difficultyLevel, uint nodeCapcityMultiplier, uint numberOfMonsters)
        {
            predicates = new Predicates();
            items = new List<Item>();
            monsterPacks = new List<Pack>();


            do
            {
                validGame = true;
                Logger.log("Creating a game of difficulty level " + difficultyLevel + ", node capacity multiplier "
                           + nodeCapcityMultiplier + ", and " + numberOfMonsters + " monsters.");
                player = new Player();
                dungeon = new Dungeon(difficultyLevel, nodeCapcityMultiplier);

                player.location = dungeon.startNode;

                if (!SeedMonsterPacks(difficultyLevel, numberOfMonsters))
                {
                    validGame = false;
                }
                if (!SeedItems())
                {
                    validGame = false;
                }

            } while (!validGame);
        }

        public Game() //empty game for tests
        {
            predicates = new Predicates();
            items = new List<Item>();
            monsterPacks = new List<Pack>();
        }

        public bool SeedMonsterPacks(uint difficultyLevel, uint numberOfMonsters)
        {
            for (int i = 1; i <= predicates.countNumberOfBridges(dungeon.startNode, dungeon.exitNode); ++i)
            {
                List<Pack> temp = CreateMonsterPacks(difficultyLevel, numberOfMonsters, (uint)i);
                foreach (Node n in predicates.reachableNodes(dungeon.startNode))
                {
                    if(i == dungeon.level(n))
                    {
                        if(temp != null && temp.Count != 0)
                        {
                            n.packs.Add(temp.Last()); //add pack to nodes
                            monsterPacks.Add(temp.Last()); //add to seperate monsterlist
                            temp.Remove(temp.Last());

                            int tempsumMonsters = 0;
                            foreach(Pack p in n.packs)
                            {
                                foreach(Monster m in p.members)
                                {
                                    tempsumMonsters++;
                                }
                            }

                            if(tempsumMonsters > difficultyLevel * (dungeon.level(n) + 1))
                            {
                                return false;
                            }
                        }                 
                    }
                }
            }


            return true;
        }

        public List<Pack> CreateMonsterPacks(uint difficultyLevel, uint numberOfMonsters, uint nodelevel)
        {
            uint monsterPackId = 0;
            List<Pack> tempMonsterPacks = new List<Pack>();

            //capacity is difficultylevel * (level of the node  + 1)
            //uint monstersLeftToPack = ((2 * nodelevel * numberOfMonsters) / ((difficultyLevel + 2) * (difficultyLevel + 1)));
            uint monstersLeftToPack = numberOfMonsters;

            while (monstersLeftToPack > 0)
            {
                uint tempAmountMonsters = 1 + (uint)RandomGenerator.rnd.Next((int)monstersLeftToPack);
                tempMonsterPacks.Add(new Pack(monsterPackId.ToString(), tempAmountMonsters));
                monstersLeftToPack -= tempAmountMonsters;
                monsterPackId++;
                Debug.Assert(monstersLeftToPack >= 0, "cant pack negative amount of monsters" + tempAmountMonsters + " " + monstersLeftToPack);
            }

            return tempMonsterPacks;
        }

        public bool SeedItems()
        {
            int tempCrystalId = 0;
            int tempPotionId = 0;
            foreach(Node n in predicates.reachableNodes(dungeon.startNode))
            {
                if(RandomGenerator.rnd.Next(24) == 0) // 1 out of 23 chance to place crystal on every node
                {
                    n.items.Add(new Crystal(tempCrystalId.ToString()));
                    items.Add(new Crystal(tempCrystalId.ToString()));
                    tempCrystalId++;
                }
                if(RandomGenerator.rnd.Next(19) == 0) // 1 out of 20 chance to place potion every node
                {
                    n.items.Add(new HealingPotion(tempPotionId.ToString()));
                    items.Add(new HealingPotion(tempPotionId.ToString()));
                    tempPotionId++;
                }
            }

            if (!PotionProperty())
                return false;

            return true;
        }

        public bool PotionProperty()
        {
            uint HPmonsters = 0;
            foreach(Pack p in monsterPacks)
            {
                foreach(Monster m in p.members)
                {
                    HPmonsters += (uint)m.HP;
                }
            }

            uint HPpotions = (uint)player.HP;
            foreach (Item p in items)
            {
                HPpotions += p.HPvalue;
            }
            foreach(Item p in player.bag)
            {
                HPpotions += p.HPvalue;
            }

            if(HPpotions < 0.8 * HPmonsters)
                return true;

            return false;
        }
        /*
         * A single update turn to the game. 
         */
        public Boolean update(Command userCommand)
        {
            Logger.log("Player does " + userCommand);
        
            return true;
        }
    }

    public class GameCreationException : Exception
    {
        public GameCreationException() { }
        public GameCreationException(String explanation) : base(explanation) { }
    }
}
