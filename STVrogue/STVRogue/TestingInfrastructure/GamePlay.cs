using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace STVRogue.GameLogic
{
    public class GamePlay
    {
        public string[] gamedata;
        public uint turn;
        public GamePlay(string filename) //load a game to gamedata
        {
            gamedata = System.IO.File.ReadAllLines(filename);
            turn = 0;
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
                    if (turn < gamedata.Length)
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
            Game temp = new Game();
            return temp;
        }

        public void Reset() // reset game to turn 0
        {
            turn = 0;
        }

        public void replayTurn() // replay current turn then advance
        {

        }
    }
}
