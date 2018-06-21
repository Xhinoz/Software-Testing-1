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
            uint diff = 6;
            uint multi = 3;
            uint monsters = 80;

            Game game = new Game(diff, multi, monsters);
            StreamWriter sw = new StreamWriter(@"..\..\..\testruns\test.txt", false);
            sw.AutoFlush = true;

            int seed = RandomGenerator.rnd.Next(10000000);
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
