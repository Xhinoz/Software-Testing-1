using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STVRogue.Utils;

namespace STVRogue.GameLogic
{
    public class Pack
    {
        public String id;
        public List<Monster> members = new List<Monster>();
        int startingHP = 0;
        public Node location;
        public Dungeon dungeon;
        public int level;

        public Pack(String id, uint n)
        {
            this.id = id;
            for (int i = 0; i < n; i++)
            {
                Monster m = new Monster("" + id + "_" + i);
                members.Add(m);
                startingHP += m.HP;
                m.pack = this;
            }
        }

        public void Attack(Player p)
        {
            foreach (Monster m in members)
            {
                m.Attack(p);
                if (p.HP == 0) break;
            }
        }

        /* Move the pack to an adjacent node. */
        public bool move(Node u)
        {
            if (!location.neighbors.Contains(u)) throw new ArgumentException();
            int capacity = (int) (dungeon.M * (dungeon.level(u) + 1));
            // count monsters already in the node:
            foreach (Pack Q in u.packs)
                capacity = capacity - Q.members.Count;           
            // capacity now expresses how much space the node has left
            if (members.Count > capacity)
            {
                Logger.log("Pack " + id + " is trying to move to a full node " + u.id + ", but this would cause the node to exceed its capacity. Rejected.");
                return false;
            }
            location.packs.Remove(this); // Remove pack from current node
            location = u;
            u.packs.Add(this);
            Logger.log("Pack " + id + " succesfully moves to " + u.id + " node.");
            return true;
        }

        /* Move the pack one node further along a shortest path to u. */
        public void moveTowards(Node u)
        {
            List<Node> path = Dungeon.shortestpath(location, u);
            move(path[1]); // 0 or 1 depending whether own node included
        }
    }
}
