using FsCheck;
using NUnit.Framework;
using STVRogue.Utils;

namespace STVRogue.GameLogic
{
    [TestFixture]
    public class NTest_Dungeon       
    {
        [Test]
        public void TestConstructor()
        {
            var p = new Predicates();
            Prop.ForAll<uint>(
                n =>
                {
                    var d = new Dungeon(n, 1);
                    return p.isValidDungeon(d.startNode, d.exitNode, n).
                    When(n <= 1000);
                }
            ).QuickCheckThrowOnFailure();
        }

    }
}
