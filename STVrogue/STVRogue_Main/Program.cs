using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STVRogue.GameLogic;
using STVRogue.Utils;
namespace STVRogue
{
    /* A dummy top-level program to run the STVRogue game */
    class Program
    {
        static void Main(string[] args)
        {
            uint diff = 5;
            uint multi = 2;
            uint monsters = 0;
            Game game = new Game(diff, multi, monsters);
            StreamWriter sw = new StreamWriter("test.txt", false);
            sw.AutoFlush = true;

            int seed = RandomGenerator.rnd.Next();
            RandomGenerator.initializeWithSeed(seed);

            sw.WriteLine(seed);
            sw.WriteLine(diff);
            sw.WriteLine(multi);
            sw.WriteLine(monsters);

            // game.player.location = new Node("a dummy node");
            while (true)
            {
                // Console.ReadKey();
                game.update(new Command(sw));
            }
            
        }
    }
}
