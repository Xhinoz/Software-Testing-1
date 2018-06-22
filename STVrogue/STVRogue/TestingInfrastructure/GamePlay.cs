using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using STVRogue.Utils;

namespace STVRogue.GameLogic
{
    public class GamePlay
    {
        Game g;
        private string filename;

        public GamePlay(string filename) //load a game to gamedata
        {
            this.filename = filename;
            Console.SetIn(new StreamReader(@"..\..\..\testruns\" + filename));
            RandomGenerator.initializeWithSeed((int)readUInt());
            g = new Game(readUInt(), readUInt(), readUInt());
        }

        private uint readUInt()
        {
            return uint.Parse(Console.ReadLine());
        }

        public bool Replay(Specification S)
        {
            UI.spec = S;
            Reset();
            while (g.update(new Command()))
                if (!UI.result)
                    return false;
            return UI.result;
        }


        public Game getState() // get an instance of the game representing the game's state
        {
            return g;
        }

        public void Reset() // reset game to turn 0
        {
            Console.SetIn(new StreamReader(@"..\..\..\testruns\" + filename));
            for (int i = 0; i < 4; i++)
                Console.ReadLine();
        }

        /*public void replayTurn() // replay current turn then advance
        {
            uint t = turn + 4; //turns start from 4 in the array
            if (gamedata[t].Contains("move"))
                command.Move(g.player, Dungeon.nodes[gamedata[t].Split(' ')[1]]);
            else if (gamedata[t].Contains("nothing"))
                command.DoNothing(g.player, null);
            else if (gamedata[t].Contains("used")) //item id to itemtype to decide which item to use
                command.UseItem(g.player, g.player.bag[Int32.Parse(gamedata[t].Split(' ')[1])].IDtotype());
            else if (gamedata[t].Contains("attack"))
                command.AttackMonster(g.player, g.LookUpMonster(gamedata[t].Split(' ')[1]));
            
            turn++;
        }*/
    }
}
