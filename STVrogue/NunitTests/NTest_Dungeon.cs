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
                    uint m = n > 0 ? n : 1;
                    var d = new Dungeon(m, 1);
                    return p.isValidDungeon(d.startNode, d.exitNode, m).
                    When(1 <= n && n <= 1000);
                }
            ).QuickCheckThrowOnFailure();
        }

    }
}
