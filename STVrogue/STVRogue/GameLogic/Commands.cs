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
            writer?.WriteLine("move " + n.id);
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
            n.fight(player, this);
        }
        public void UseItem(Player player, int item)
        {
            string item_id = player.useBagItem(item);
            if (item == 1)
                Console.WriteLine("{0} has used a Healing Potion.", player.name);
            else
                Console.WriteLine("{0} has used a Crystal.");
            writer?.WriteLine("useitem {0}", item_id);
        }
        public void DoNothing(Player player, Node location) // Possible to remove location parameter
        {
            writer?.WriteLine("nothing");
            Console.WriteLine("{You decided to rest and not move.");
            player.location.fight(player, this);
            //location.fight(player, this);
        }
        public void AttackMonster(Player player, Monster monster)
        {
            if(!player.accelerated)
                writer?.WriteLine("attack {0}", monster.id);
            else
                writer?.WriteLine("attackcrystalized {0}", monster.id);

            player.Attack(monster);
        }
    }
}
