using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using STVRogue.Utils;

namespace STVRogue.GameLogic
{
    public class Dungeon
    {
        public Node startNode;
        public Node exitNode;
        public uint difficultyLevel;
        /* a constant multiplier that determines the maximum number of monster-packs per node: */
        public uint M;
        public Bridge[] bridges;
        public static uint counter;
        public static Dictionary<string, Node> nodes = new Dictionary<string, Node>();
        public static Dungeon current;
        private Random rng;
        private Predicates p = new Predicates();
        public static int alert = 0; // Alarm level

        /* To create a new dungeon with the specified difficult level and capacity multiplier */
        public Dungeon(uint level, uint nodeCapacityMultiplier)
        {
            if (level == 0)
                throw new ArgumentException("Dungeon level must be at least 1.");
            Logger.log("Creating a dungeon of difficulty level " + level + ", node capacity multiplier " + nodeCapacityMultiplier + ".");
            current = this;
            difficultyLevel = level;
            M = nodeCapacityMultiplier;
            bridges = new Bridge[level];
            rng = RandomGenerator.rnd;
            startNode = new Node("start");
            int nodes, conns;
            
            do
            {
                counter = level + 2;
                Node start = startNode;
                int startc = rng.Next(1, 5);
                nodes = 1;
                conns = 0;
                int i = 1;
                while (i <= level + 1)
                {
                    var result = makeSection(start, startc, ref i);
                    if (i <= level + 1) start = bridges[i - 2];
                    startc = result.Item1;
                    nodes += result.Item2;
                    conns += result.Item3;
                }
            } while (conns / (double)nodes > 3);
        }

        /*Tuple contains the remaining connections from the bridge,
         the amount of nodes, and the total amount of connections.*/
        private Tuple<int, int, int> makeSection(Node start, int startc, ref int level)
        {
            Bridge b = new Bridge(level.ToString());
            Bridge bstart = start as Bridge;
            if (startc == 1) //can only be directly connected to a bridge
            {
                level++;
                b.connectToNodeOfSameZone(start);
                if (bstart != null)
                {
                    bstart.connectToNodeOfNextZone(b);
                    bstart.disconnect(b);
                }
                if (level > difficultyLevel + 1)
                    exitNode = new Node(b);
                else bridges[level - 2] = b;
                return Tuple.Create(rng.Next(1, 4), 1, 1);
            }
            HashSet<int> open = new HashSet<int>();
            List<Node> nodes = new List<Node>();
            List<int> conns = new List<int>();
            int totalConns = 0;
            open.Add(0);
            nodes.Add(start);
            conns.Add(startc);
            int index;
            while (open.Count > 1 || open.Contains(0))
            {
                Node n = new Node();
                index = nodes.Count;
                open.Add(index);
                nodes.Add(n);
                conns.Add(rng.Next(1, 5));
                for (int i = 0; i < index; i++)
                    if (open.Contains(i) && conns.Sum() > open.Count)
                    {
                        totalConns++;
                        nodes[i].connect(n);
                        conns[i]--;
                        if (conns[i] == 0) open.Remove(i);
                        conns[index]--;
                        if (conns[index] == 0)
                        {
                            open.Remove(index);
                            break;
                        }
                    }
            }
            if (bstart != null)
                bstart.toNodes = bstart.neighbors.Except(bstart.fromNodes).ToList();
            index = open.Single();
            foreach (Node nd in shortestpath(start, nodes[index])
                .Where(m => m == nodes[index] || p.isBridge(start, nodes[index], m)))
            {
                level++;
                if (level > difficultyLevel + 1)
                {
                    exitNode = nd;
                    exitNode.id = "exit";
                    break;
                }
                else
                {
                    var nb = nd.neighbors;
                    while (nb.Count > 0)
                    {
                        Node m = nb[0];
                        m.disconnect(nd);
                        if (p.isReachable(start, m))
                            b.connectToNodeOfSameZone(m);
                        else
                            b.connectToNodeOfNextZone(m);
                    }
                    bridges[level - 2] = b;
                    b = new Bridge(level.ToString());
                }
            }
            return Tuple.Create(conns[index], nodes.Count - 1, totalConns);
        }

        /* Return a shortest path between node u and node v */
        /*Changed to static to make it easier to insert your own graphs without
         having to generate a dungeon, hope that isn't a problem.*/
        public static List<Node> shortestpath(Node u, Node v)
        {
            var queue = new Queue<Node>();
            var dict = new Dictionary<Node, Node>();
            var visited = new HashSet<Node>();
            queue.Enqueue(v);
            while (queue.Count > 0)
            {
                Node n = queue.Dequeue();
                foreach (Node m in n.neighbors)
                    if (!visited.Contains(m))
                    {
                        dict[m] = n;
                        if (m == u)
                        {
                            var result = new List<Node>();
                            Node a = u;
                            while (a != v)
                            {
                                result.Add(a);
                                a = dict[a];
                            }
                            result.Add(v);
                            return result;
                        }
                        queue.Enqueue(m);
                        visited.Add(m);
                    }
            }
            return null;
        }


        /* To disconnect a bridge from the rest of the zone the bridge is in. */
        public void disconnect(Bridge b)
        {
            Logger.log("Disconnecting the bridge " + b.id + " from its zone.");
            b.neighbors = b.neighbors.Except(b.fromNodes).ToList();
            startNode = new Node(b);
        }

        /* To calculate the level of the given node. */
        public uint level(Node d) {
            if (d is Bridge)
                return uint.Parse(d.id);
            return 0;
        }

        /*To calculate the actual level without the weird "every node that isn't a Bridge is 0" rule*/
        public uint nodeLevel(Node d)
        {
            uint level = g.dungeon.level(Dungeon.shortestpath(d, g.dungeon.exitNode).FirstOrDefault(n => n is Bridge));
            if (level == 0)
                level = g.dungeon.difficultyLevel + 1;
            return level;
        }

        /*To calculate the capacity of a node*/
        public uint capacity(Node d)
        {
            return M * (nodeLevel(d) + 1);
        }
    }

    public class Node
    {
        public String id;
        public List<Node> neighbors = new List<Node>();
        public List<Pack> packs = new List<Pack>();
        public List<Item> items = new List<Item>();

        public Node() : this(Dungeon.counter++.ToString()) { }
        public Node(String id)
        {
            this.id = id;
            if (Dungeon.nodes != null)
                Dungeon.nodes[id] = this;
        }
        
        /*Because the exit node being a bridge is unacceptable.*/
        public Node(Bridge b)
        {
            id = b.id;
            while (b.neighbors.Count > 0)
            {
                connect(b.neighbors[0]);
                b.disconnect(b.neighbors[0]);
            }
            packs = b.packs;
            items = b.items;
        }

        /* To connect this node to another node. */
        public void connect(Node nd)
        {
            neighbors.Add(nd); nd.neighbors.Add(this);
        }

        /* To disconnect this node from the given node. */
        public void disconnect(Node nd)
        {
            neighbors.Remove(nd); nd.neighbors.Remove(this);
        }

        /* Execute a fight between the player and the packs in this node.
         * Such a fight can take multiple rounds as describe in the Project Document.
         * A fight terminates when either the node has no more monster-pack, or when
         * the player's HP is reduced to 0. 
         */
        public void fight(Player player, Command commands)
        {
            while (player.location == this && packs.Count != 0) // Contested
            {
                Dungeon.alert = player.level;

                // Choice
                Console.WriteLine("A foe stands infront of you!");
                Console.WriteLine("You have {0} health.", player.HP);
                Console.WriteLine("1) Flee. \n2) Use item in your inventory and attack. \n3) Attack a monster.");
                // Flee, item > attack, attack
                int choice = int.Parse(Console.ReadKey().KeyChar.ToString());

                switch (choice)
                { // Flee
                    case 1:
                        // Choice
                        Game.DisplayPaths(player);
                        choice = int.Parse(Console.ReadKey().KeyChar.ToString());
                        Node destination = neighbors[choice - 1];
                        commands.Move(player, destination);
                        break;
                    // Use item
                    case 2:
                        // Choice
                        int bag = player.DisplayInventory();
                        Console.WriteLine("1) Healingpotion \n2) Crystal");
                        choice = int.Parse(Console.ReadKey().KeyChar.ToString());
                        if (choice == 1 && (bag == 1 || bag == 3))
                            commands.UseItem(player, choice);
                        else if (choice == 2 && (bag == 2 || bag == 3))
                            commands.UseItem(player, choice);
                        else if (choice != 1 && choice != 2) 
                        {
                            Console.WriteLine("Wrong input.");
                                goto case 2; // Repeater
                        }
                        else // No items in inventory
                        {
                            Console.WriteLine("You don't have the item in your inventory!");
                            Console.WriteLine("1) Flee \n2) Attack");
                            choice = int.Parse(Console.ReadKey().KeyChar.ToString());
                            if (choice == 1)
                                goto case 1;
                        }

                        goto case 3; // Continue to Attack

                    // Attack
                    case 3:
                        // GUI Monster choice 
                        Console.WriteLine("Choose the pack and monster you want to attack.");
                        Game.DisplayPacks(packs);
                        int pack_choice = int.Parse(Console.ReadKey().KeyChar.ToString()) - 1; // 0 indexed
                        Game.DisplayMonsters(packs[pack_choice].members);
                        int monster_choice = int.Parse(Console.ReadKey().KeyChar.ToString()) - 1;
                        Monster monster = packs[pack_choice].members[monster_choice];
                        commands.AttackMonster(player, monster);

                        if (packs[pack_choice].members.Count == 0) // Pack died
                            packs.RemoveAt(pack_choice);


                        if (packs.Count != 0) // Monster Turn; Still Contested
                        {
                            int rand_pack = RandomGenerator.rnd.Next(packs.Count);
                            int HP = 0;
                            int totalHP = 0;
                            foreach (Monster monst in packs[rand_pack].members)
                            {
                                HP += monst.HP;
                                totalHP += monst.totalHP;
                            }
                            double flee_chance = (1 - HP / totalHP) / 2;
                            bool flee = RandomGenerator.rnd.NextDouble() < flee_chance;

                            if (flee) // Fleeing
                            {
                                bool fled = false;
                                int tried = 0;
                                List<Node> temp_neighbours = new List<Node>(neighbors); // 
                                while (!fled && tried != neighbors.Count) // Randomly chooses neighbours and tries to move
                                {
                                    int rand_neighbour = RandomGenerator.rnd.Next(temp_neighbours.Count);
                                    if (Game.RZone(packs[rand_pack], temp_neighbours[rand_neighbour])) // Check to stay in zone 
                                    {
                                        fled = packs[rand_pack].move(temp_neighbours[rand_neighbour]);
                                    }

                                    temp_neighbours.RemoveAt(rand_neighbour);
                                    tried++;
                                }

                                if (!fled) // Not succesfully fled
                                    packs[rand_pack].Attack(player);
                                else
                                {
                                    if (packs.Count != 0) // Still contested
                                    {
                                        rand_pack = RandomGenerator.rnd.Next(packs.Count);
                                        packs[rand_pack].Attack(player);
                                    }
                                }
                            }
                            else // Originally not fleeing                          
                                packs[rand_pack].Attack(player);

                        }

                        break;
                }
            }
        }
    }

    public class Bridge : Node
    {
        public List<Node> fromNodes = new List<Node>();
        public List<Node> toNodes = new List<Node>();
        public Bridge(String id) : base(id) { }

        /* Use this to connect the bridge to a node from the same zone. */
        public void connectToNodeOfSameZone(Node nd)
        {
            base.connect(nd);
            fromNodes.Add(nd);
        }

        /* Use this to connect the bridge to a node from the next zone. */
        public void connectToNodeOfNextZone(Node nd)
        {
            base.connect(nd);
            toNodes.Add(nd);
        }
    }
}
