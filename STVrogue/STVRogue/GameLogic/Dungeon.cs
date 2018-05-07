using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        /* To create a new dungeon with the specified difficult level and capacity multiplier */
        public Dungeon(uint level, uint nodeCapacityMultiplier)
        {
            Logger.log("Creating a dungeon of difficulty level " + level + ", node capacity multiplier " + nodeCapacityMultiplier + ".");
            difficultyLevel = level;
            M = nodeCapacityMultiplier;
            throw new NotImplementedException();
        }

        /* Return a shortest path between node u and node v */
        public List<Node> shortestpath(Node u, Node v)
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
        public void fight(Player player)
        {
            throw new NotImplementedException();
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
