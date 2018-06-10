using System;
namespace STVRogue.GameLogic
{
    public class Command
    {
        public Command() { }
        override public string ToString() { return "no-action"; }

        public void Move(Player player, Node n)
        {
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
            Console.WriteLine("{0} has decided to rest and not move.", player.name);
        }
    }
}
