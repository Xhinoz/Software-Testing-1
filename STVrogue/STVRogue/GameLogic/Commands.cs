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
            n.fight(player);
        }
        public void UseItem(Player player, Item item)
        {

            player.use(item);
            Console.WriteLine("{0} has used a Healing Potion.", player.name);
        }
        public void DoNothing(Player player)
        {
            writer?.WriteLine("nothing");
            Console.WriteLine("{0} has decided to rest and not move.", player.name);
        }
    }
}
