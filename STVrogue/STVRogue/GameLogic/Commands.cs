using System;
using System.IO;

namespace STVRogue.GameLogic
{
    public class Command
    {
        StreamWriter writer;

        public Command() { }
        public Command(StreamWriter writer) { this.writer = writer; }
        override public string ToString() { return "no-action"; }

        public void Move(Player player, Node n)
        {
            // writer.WriteLine("move " + n.id);
            if (player.location.GetType().Name == "Bridge") // Up player level when entering new zone
            {
                Bridge bridge = player.location as Bridge;
                if (bridge.toNodes.Contains(n))
                {
                    player.level++;
                    Dungeon.alert = 0; // Reset alarm
                }
            }
            player.location = n;
            Console.WriteLine("{0} moved to {1}.", player.name, n.id);
            //n.fight(player);
        }
        public void UseItem(Player player, int item)
        {
             string item_id = player.useBagItem(item);
            //player.use(item);
            Console.WriteLine("{0} has used a Healing Potion.", player.name);
            //writer.WriteLine("useitem {0}", item_id);
        }
        public void DoNothing(Player player, Node location)
        {
            //writer.WriteLine("nothing");
            Console.WriteLine("{0} has decided to rest and not move.", player.name);
            //location.fight(player);
        }
        public void AttackMonster(Player player, Monster monster)
        {
            player.Attack(monster);
            // crystal scenario
            if (player.accelerated)
            {
                string text = "";
                int amount = monster.pack.members.Count;
                text += amount;
                foreach (Monster mon in monster.pack.members)
                    text += " " + mon.id;

                // writer.WriteLine("attackmultiple {0}", text);
            }
            //else
            //    writer.WriteLine("attack {0}, monster.id");
        }
    }
}
