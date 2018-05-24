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
        private Bridge[] bridges;
        private Random rng;

        /* To create a new dungeon with the specified difficult level and capacity multiplier */
        public Dungeon(uint level, uint nodeCapacityMultiplier)
        {
            if (level == 0)
                throw new ArgumentException("Dungeon level must be at least 1.");
            Logger.log("Creating a dungeon of difficulty level " + level + ", node capacity multiplier " + nodeCapacityMultiplier + ".");
            difficultyLevel = level;
            M = nodeCapacityMultiplier;
            bridges = new Bridge[level];
            //TODO: Remove seed when testing is done.
            rng = new Random(42);
            startNode = new Node();
            int nodes, conns;
            do
            {
                Node start = startNode;
                int startc = rng.Next(1, 5);
                nodes = 1;
                conns = 0;
                for (int i = 1; i <= level + 1; i++)
                {
                    var result = makeSection(start, startc, level);
                    if (i <= level) start = bridges[level - 1];
                    startc = result.Item1;
                    nodes += result.Item2;
                    conns += result.Item3;
                }
            } while (conns / (double)nodes > 3);
        }

        /*Tuple contains the remaining connections from the bridge,
         the amount of nodes, and the total amount of connections.*/
        private Tuple<int, int, int> makeSection(Node start, int startc, uint level)
        {
            Bridge b = new Bridge(level.ToString());
            if (startc == 1) //can only be directly connected to a bridge
            {
                b.connectToNodeOfSameZone(start);
                (start as Bridge)?.connectToNodeOfNextZone(b);
                if (level > difficultyLevel)
                    exitNode = new Node(b);
                else bridges[level - 1] = b;
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
                    if (open.Contains(i) && (open.Count > 2 || conns[i] + conns[index] > 2))
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
            if (start is Bridge bstart)
                bstart.toNodes = bstart.neighbors.Except(bstart.fromNodes).ToList();
            index = open.Single();
            var nb = nodes[index].neighbors;
            while (nb.Count > 0)
            {
                b.connectToNodeOfSameZone(nb[0]);
                nb[0].disconnect(nodes[index]);
            }
            if (level > difficultyLevel)
                exitNode = new Node(b);
            else bridges[level - 1] = b;
            return Tuple.Create(conns[index], nodes.Count - 1, totalConns);
        }

        /* Return a shortest path between node u and node v */
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
            b.fromNodes.Clear();
            startNode = b;
        }

        /* To calculate the level of the given node. */
        public uint level(Node d) {
            if (d is Bridge)
            {
                for (uint i = 0; i < bridges.Length; i++)
                    if (bridges[i] == d)
                        return i + 1;
            }
            return 0;
        }
    }

    public class Node
    {
        public String id;
        public List<Node> neighbors = new List<Node>();
        public List<Pack> packs = new List<Pack>();
        public List<Item> items = new List<Item>();

        public Node() { }
        public Node(String id) { this.id = id; }
        
        /*Because the exit node being a bridge is unacceptable.*/
        public Node(Bridge b)
        {
            id = b.id;
            neighbors = b.neighbors;
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
        public void fight(Player player, int choice)
        {
            while (player.location == this && packs.Count != 0) // Contested
            {
                // Choice?
                // int choice = int.Parse(Console.ReadLine());
                //int choice = RandomGenerator.rnd.Next(3);

                switch (choice)
                { // Flee
                    case 0:
                        // Choice?
                        int random = RandomGenerator.rnd.Next(neighbors.Count);
                        Node destination = neighbors[random];
                        player.location = destination;
                        break;
                    // Use item
                    case 1:
                        // Choice?
                        //bool potion = false;
                        //int potion_pos;
                        //bool crystal = false;
                        //int crystal_pos;
                        //for (int t = 0; t < player.bag.Count; t++)
                        //{
                        //    if (player.bag[t].GetType().Name == "HealingPotion")
                        //    {
                        //        potion = true;
                        //        potion_pos = t;
                        //    }
                        //    if (player.bag[t].GetType().Name == "Crystal")
                        //    {
                        //        crystal = true;
                        //        crystal_pos = t;
                        //    }
                        //}
                        //Item item = player.bag[t];
                        if (player.bag.Count != 0)
                        {
                            int rand = RandomGenerator.rnd.Next(player.bag.Count);
                            Item item = player.bag[rand];
                            player.use(item);
                        }
                        goto case 2; // Continue to Attack

                    // Attack
                    case 2:
                        // Choice?
                        int rand_pack = RandomGenerator.rnd.Next(packs.Count);
                        int rand_monster = RandomGenerator.rnd.Next(packs[rand_pack].members.Count);
                        Monster monster = packs[rand_pack].members[rand_monster];
                        player.Attack(monster);

                        if (packs[rand_pack].members.Count == 0) // Pack died
                            packs.RemoveAt(rand_pack);

                        if (packs.Count != 0) // Monster Turn; Still Contested
                        {
                            rand_pack = RandomGenerator.rnd.Next(packs.Count);
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
                                while (!fled || tried != neighbors.Count) // Randomly chooses neighbours and tries to move
                                {
                                    int rand_neighbour = RandomGenerator.rnd.Next(temp_neighbours.Count);
                                    fled = packs[rand_pack].move(temp_neighbours[rand_neighbour]);
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
