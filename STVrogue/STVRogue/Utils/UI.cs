using STVRogue.GameLogic;
using System;
using System.IO;

namespace STVRogue.Utils
{
    public static class UI
    {
        public static StreamWriter writer;
        public static Specification spec;
        public static bool result = true;

        public static char ReadKey()
        {
            if (writer == null)
            {
                int x = Console.Read();
                if (x == -1)
                    Game.lastTurn = true;
                result &= spec.test(Game.game);
                return (char)x;
            }
            else
            {
                char c = Console.ReadKey().KeyChar;
                writer.Write(c);
                return c;
            }
            
        }
    }
}
