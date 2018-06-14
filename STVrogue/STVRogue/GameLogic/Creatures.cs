using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STVRogue.Utils;

namespace STVRogue.GameLogic
{
    public class Creature
    {
        public String id;
        public String name;
        public int HP;
        public uint AttackRating = 1;
        public Node location;
        public Creature() { }
        virtual public void Attack(Creature foe)
        {
            foe.HP = (int)Math.Max(0, foe.HP - AttackRating);
            String killMsg = foe.HP == 0 ? ", KILLING it" : "";
            Logger.log("Creature " + id + " attacks " + foe.id + killMsg + ".");
        }
    }
    
    public class Monster : Creature
    {
        public Pack pack;
        public int totalHP;

        /* Create a monster with a random HP */
        public Monster(String id)
        {
            this.id = id; name = "Orc";
            HP = 1 + RandomGenerator.rnd.Next(6);
            totalHP = HP;
        }
    }

    public class Player : Creature
    {
        public Dungeon dungeon;
        public int HPbase = 100;
        public Boolean accelerated = false;
        public uint KillPoint = 0;
        public List<Item> bag = new List<Item>();
        public int level = 1;
        // For Testing
        private List<int> commands = new List<int>();
        private int commanditerator = 0;
        public Player()
        {
            id = "player";
            AttackRating = 5;
            HP = HPbase;
        }

        public void use(Item item)
        {
            if (!bag.Contains(item) || item.used) throw new ArgumentException();
            item.use(this);
            bag.Remove(item);
        }

        override public void Attack(Creature foe)
        {
            if (!(foe is Monster)) throw new ArgumentException();
            Monster foe_ = foe as Monster;
            if (!accelerated)
            {
                base.Attack(foe);
                if (foe_.HP == 0)
                {
                    foe_.pack.members.Remove(foe_);
                    KillPoint++;
                }
            }
            else
            {
                int packCount2 = foe_.pack.members.Count;
                for (int i = packCount2 - 1; i >= 0; i--)
                {
                    Monster target = foe_.pack.members[i];
                    base.Attack(target);
                    if (target.HP == 0)
                    {
                        target.pack.members.Remove(target);
                        KillPoint++;
                    }
                }
                accelerated = false;
            }
        }
        // For testing 
        public void AddNextCommand(int choice)
        {
            commands.Add(choice);
        }
        public void AddNextCommand(params int[] array)
        {
            for (int i = 0; i < array.Length; i++)
                commands.Add(array[i]);
        }
        public int GetNextCommand()
        {
            return commands[commanditerator++];
        }
    }
}
