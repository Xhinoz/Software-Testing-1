using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STVRogue.GameLogic
{
    public class Specification
    {
        public bool test(Game g) { return true; }
    }

    public class NonNegativeHP_Spec : Specification
    {
        public bool test(Game g) { return g.player.HP >= 0; }
    }

    
}
