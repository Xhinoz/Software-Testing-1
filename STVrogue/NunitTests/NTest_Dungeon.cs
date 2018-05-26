using FsCheck;
using NUnit.Framework;
using STVRogue.Utils;
using System;

namespace STVRogue.GameLogic
{
    [TestFixture]
    public class NTest_Dungeon
    {
        Predicates p = new Predicates();

        [Test]
        public void TestConstructor()
        {
            Prop.ForAll<uint>(
                n =>
                {
                    uint m = n > 0 ? n : 1;
                    var d = new Dungeon(m, 1);
                    return p.isValidDungeon(d.startNode, d.exitNode, m).
                    When(1 <= n);
                }
            ).QuickCheckThrowOnFailure();
        }

        [Test]
        public void FailConstructor()
        {
            Assert.Throws<ArgumentException>(() => new Dungeon(0, 1));
        }

        [Test]
        public void TestDisconnect()
        {
            Prop.ForAll<uint, uint>(
                (a, b) =>
                {
                    var dandb = getDungeonAndBridge(a, b);
                    var d = dandb.Item1;
                    var br = dandb.Item2;
                    uint lvl = 1;
                    if (br != null)
                    {
                        d.disconnect(br);
                        lvl = b - a - 1;
                    }
                    return p.isValidDungeon(d.startNode, d.exitNode, lvl)
                    .When(br != null);
                }
            ).QuickCheckThrowOnFailure();
        }

        [Test]
        /*Removing the neighbors from a bridge of a lower level means the higher
         bridge will only be reachable from the end and not from the start.
         It's the other way around for removing neighbors from a bridge of a higher level.
         This method tests both of these properties to verify the given level is correct.*/
        public void TestLevel()
        {
            Prop.ForAll<uint, uint, uint>(
                (a, b, c) =>
                {
                    var dandb = getDungeonAndBridge(a, b);
                    var d = dandb.Item1;
                    var br = dandb.Item2;
                    if (br != null && c < a)
                    {
                        var br2 = d.bridges[c];
                        foreach (Node n in br.neighbors)
                            n.neighbors.Remove(br);
                        bool lvlHigh = d.level(br2) > d.level(br);
                        bool fromExit = p.isReachable(br2, d.exitNode);
                        bool fromStart = p.isReachable(br2, d.startNode);
                        return ((lvlHigh && fromExit && !fromStart) ||
                            (!lvlHigh && !fromExit && fromStart))
                            .When(br != br2);
                    }
                    return false.When(false);
                }
            ).QuickCheckThrowOnFailure();
        }

        /*Just to easily get a bridge in a dungeon.*/
        private Tuple<Dungeon, Bridge> getDungeonAndBridge(uint a, uint b)
        {
            if (a <= b) return Tuple.Create<Dungeon, Bridge>(new Dungeon(1, 1), null);
            uint m = a > 0 ? a : 1;
            var d = new Dungeon(m, 1);
            Bridge br = d.bridges[b];
            return Tuple.Create(d, br);
        }
    }
}
