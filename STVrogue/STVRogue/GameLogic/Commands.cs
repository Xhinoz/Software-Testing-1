using System;
using System.Collections.Generic;
using System.IO;

namespace STVRogue.GameLogic
{
    public class Command
    {
        override public string ToString() { return "no-action"; }

        public void Move(Player player, Node n)
        {
            if (player.location is Bridge bridge) // Up player level when entering new zone
                if (bridge.toNodes.Contains(n))
                {
                    player.level++;
                    Dungeon.alert = 0; // Reset alarm
                }
            player.location = n;
            // Pick up items and remove from node
            foreach (Item item in n.items)
                player.bag.Add(item);
            n.items.Clear();

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
        }
        public void DoNothing(Player player, Node location) // Possible to remove location parameter
        {
            Console.WriteLine("You decided to rest and not move.");
            player.location.fight(player, this);
            //location.fight(player, this);
        }
        public void AttackMonster(Player player, Monster monster)
        {
            player.Attack(monster);
        }
    }
}
