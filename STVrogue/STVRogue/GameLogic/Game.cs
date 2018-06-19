using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STVRogue.Utils;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

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
        public bool lastTurn;

        /* This creates a player and a random dungeon of the given difficulty level and node-capacity
         * The player is positioned at the dungeon's starting-node.
         * The constructor also randomly seeds monster-packs and items into the dungeon. The total
         * number of monsters are as specified. Monster-packs should be seeded as such that
         * the nodes' capacity are not violated. Furthermore the seeding of the monsters
         * and items should meet the balance requirements stated in the Project Document.
         */

        //create a game until a valid one is made
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

                if (!SeedMonsterPacks2(difficultyLevel, numberOfMonsters))
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


        //create random packs then loop
        public bool SeedMonsterPacks2(uint difficultyLevel, uint numberOfMonsters)
        {
            List<Pack> temp = CreateMonsterPacks(difficultyLevel, numberOfMonsters);

            if (numberOfMonsters == 0)
                return true;

            foreach (Node n in predicates.reachableNodes(dungeon.startNode))
            {
                for (int i = 1; i <= predicates.countNumberOfBridges(dungeon.startNode, dungeon.exitNode) + 1; ++i)
                {
                    if (predicates.countNumberOfBridges(dungeon.startNode, n) + 1 == i)
                    {

                        n.packs.Add(temp.Last()); //add pack to nodes
                        monsterPacks.Add(temp.Last()); //add to seperate monsterlist
                        Pack pack = temp.Last();
                        pack.level = i; // give zone level to pack
                        pack.location = n; // add location to pack
                        pack.dungeon = dungeon; // add dungeon to pack
                        temp.Remove(temp.Last());

                        int tempsumMonsters = 0;
                        foreach (Pack p in n.packs)
                        {
                            tempsumMonsters += p.members.Count();
                        }

                        if (tempsumMonsters > difficultyLevel * (predicates.countNumberOfBridges(dungeon.startNode, n) + 1))
                        {

                            return false;
                        }

                        if (temp.Count == 0)
                            break;
                    }

                }

                if (temp.Count == 0)
                    break;
            }

            return true;
        }

        //create a list of random sized packs
        public List<Pack> CreateMonsterPacks(uint difficultyLevel, uint numberOfMonsters)
        {
            uint monsterPackId = 0;
            List<Pack> tempMonsterPacks = new List<Pack>();

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

        //seed items random percentage change to drop per node (chosen randomly)
        public bool SeedItems()
        {
            int tempId = 0;
            foreach (Node n in predicates.reachableNodes(dungeon.startNode))
            {
                if (RandomGenerator.rnd.Next(24) == 0) // 1 out of 23 chance to place crystal on every node
                {
                    n.items.Add(new Crystal(tempId.ToString()));
                    items.Add(new Crystal(tempId.ToString()));
                    tempId++;
                }
                if (RandomGenerator.rnd.Next(19) == 0) // 1 out of 20 chance to place potion every node
                {
                    n.items.Add(new HealingPotion(tempId.ToString()));
                    items.Add(new HealingPotion(tempId.ToString()));
                    tempId++;
                }
            }

            if (!PotionProperty())
                return false;

            return true;
        }

        public bool PotionProperty()
        {
            uint HPmonsters = 0;
            foreach (Pack p in monsterPacks)
            {
                foreach (Monster m in p.members)
                {
                    HPmonsters += (uint)m.HP;
                }
            }

            uint HPpotions = (uint)player.HP;
            foreach (Item p in items)
            {
                HPpotions += p.HPvalue;
            }
            foreach (Item p in player.bag)
            {
                HPpotions += p.HPvalue;
            }

            if (HPpotions < 0.8 * HPmonsters ||HPmonsters == 0)
                return true;

            return false;
        }
        /*
         * A single update turn to the game. 
         */
        public Boolean update(Command userCommand)
        {
            // Logger.log("Player does " + userCommand);
            //// Player Action /////
            GUI(userCommand);

            // Cleans up all monsters that died
            for (int t = monsterPacks.Count - 1; t >= 0; t--)
                if (monsterPacks[t].members.Count == 0)
                    monsterPacks.Remove(monsterPacks[t]);

            //// Monster Actions /////
            foreach (Pack pack in monsterPacks)
            {                   
                // move or do nothing
                bool moving = RandomGenerator.rnd.NextDouble() > 0.5;
                if (moving)
                {
                    if (REndZone(pack)) // EndZone rule activated
                        pack.moveTowards(player.location);
                    else if (RAlert(pack)) // if pack is in alarmed zone
                        pack.moveTowards(player.location);           
                    else
                    {
                        bool moved = false;
                        int tried = 0;
                        List<Node> temp_neighbours = new List<Node>(pack.location.neighbors);
                        while (!moved && tried != pack.location.neighbors.Count) // tries till success
                        {
                            int destination = RandomGenerator.rnd.Next(temp_neighbours.Count);
                            Node node_destination = temp_neighbours[destination];
                            if (RZone(pack, node_destination)) // Stays in zone
                            {
                                moved = pack.move(node_destination);
                            }
                            temp_neighbours.RemoveAt(destination);
                            tried++;
                        }
                    }
                }

            }
            return true;
        }
        // Valid destination in zone
        public static bool RZone(Pack pack, Node destination)
        {
            if (destination.GetType().Name == "Bridge") // Lower bridge
                if (Dungeon.current.level(destination) != pack.level)
                    return false;

            if (pack.location.GetType().Name == "Bridge") // Upper bridge 
                if (pack.level == Dungeon.current.level(pack.location)) // Redundant?
                {
                    Bridge bridge = pack.location as Bridge;
                    if (bridge.toNodes.Contains(destination))
                        return false;
                }

            return true;
        }
        // Which packs act on Alert
        public bool RAlert(Pack pack)
        {
            if (Dungeon.alert != 0) // Alarm raised
                if (pack.level == Dungeon.alert)
                    return true;

            return false;
        }
        // When Endzone reached
        public bool REndZone(Pack pack)
        {
            int bridges = (int) predicates.countNumberOfBridges(dungeon.startNode, dungeon.exitNode);
            if (player.level == bridges + 1) // Zone level starts at 1
                if (pack.level == player.level)
                    return true;
            
            return false;
        }

        public void GUI(Command command)
        {
            Console.WriteLine("What would you like to do next? \n1) Move to a node. \n2) Use a healing potion. \n3) Do nothing.");
            ConsoleKeyInfo info = Console.ReadKey();
            int number;
            switch (info.KeyChar)
            {
                case '1':
                    // display neighbour nodes
                    DisplayPaths(player);
                    info = Console.ReadKey();
                    number = int.Parse(info.KeyChar.ToString());
                    Node destination = player.location.neighbors[number - 1]; // Using 1-9, not 0-9
                    command.Move(player, destination);
                    break;

                case '2':
                    // display inventory
                    player.DisplayInventory();
                    Console.WriteLine("1) Use Healingpotion.");
                    info = Console.ReadKey();
                    number = int.Parse(info.KeyChar.ToString());
                    if (number == 1)   // use command item       
                        command.UseItem(player, number);                  
                    else
                    {
                        Console.WriteLine("Wrong input.");
                        goto case '2'; // Repeating
                    }          
                    break;
                case '3':
                    command.DoNothing(player, player.location);
                    break;
                case 'x':
                    // previous menu always
                    break;
                default:
                    command.DoNothing(player, player.location);
                    break;
            }
        }

        public static void DisplayPaths(Player player)
        {
            for (int t = 0; t < player.location.neighbors.Count; t++)
            {
                string text = "";
                text += (t + 1) + ") Node " + player.location.neighbors[t].id + ", ";
                text += "Path length to exit is " + Dungeon.shortestpath(player.location.neighbors[t], Dungeon.current.exitNode).Count + ".";
                Console.WriteLine(text);
            }
        }
        public static void DisplayPacks(List<Pack> packs)
        {
            for (int t = 0; t < packs.Count; t++)
            {
                int healthpool = 0;
                foreach (Monster monster in packs[t].members)
                    healthpool += monster.HP;

                string text = "";
                text = (t + 1) + ") Pack " + packs[t].id + " has a total remaining healthpool of " + healthpool;
                Console.WriteLine(text);
            }
        }
        public static void DisplayMonsters(List<Monster> monsters)
        {
            for (int t = 0; t < monsters.Count; t++)
            {
                string text = "";
                text = (t + 1) + ") Monster " + monsters[t].id + " has " + monsters[t].HP + " health.";
                Console.WriteLine(text);
            }
        }
    }

    public class GameCreationException : Exception
    {
        public GameCreationException() { }
        public GameCreationException(String explanation) : base(explanation) { }
    }
}
