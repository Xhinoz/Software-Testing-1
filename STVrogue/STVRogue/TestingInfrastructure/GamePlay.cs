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
        public string[] gamedata;
        public uint turn;
        Game g;
        Command command;
        public GamePlay(string filename) //load a game to gamedata
        {
            command = new Command();
            gamedata = System.IO.File.ReadAllLines(filename); //0 = seed , 1 2 3 init variables for game
            turn = 0;
            RandomGenerator.initializeWithSeed(Int32.Parse(gamedata[0]));
            Game g = new Game(UInt32.Parse(gamedata[1]), UInt32.Parse(gamedata[2]), UInt32.Parse(gamedata[3]));
        }



        public bool Replay(Specification S)
        {
            Reset();
            while(true)
            {
                //test specifation holds in current state
                bool ok = S.test(getState());

                //if specification holds true continue to next turn
                if(ok)
                {
                    if (turn < gamedata.Length - 4)
                        replayTurn();
                    else
                        break;
                
                }
                else
                {
                    return false;
                }
            }
            return true;
        }


        public Game getState() // get an instance of the game representing the game's state
        {
            return g;
        }

        public void Reset() // reset game to turn 0
        {
            turn = 0;
        }

        public void replayTurn() // replay current turn then advance
        {
            uint t = turn + 4; //turns start from 4 in the array
            if (gamedata[t].Contains("move"))
            {
                command.Move(g.player, Dungeon.nodes[gamedata[t].Split(' ')[1]]);
            }
            else if (gamedata[t].Contains("nothing"))
                command.DoNothing(g.player, null);
            else if (gamedata[t].Contains("used")) //item id to itemtype to decide which item to use
                command.UseItem(g.player, g.player.bag[Int32.Parse(gamedata[t].Split(' ')[1])].IDtotype());
                
            turn++;
        }
    }
}
