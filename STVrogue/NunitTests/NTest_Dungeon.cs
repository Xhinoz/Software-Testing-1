using FsCheck;
using NUnit.Framework;
using STVRogue.Utils;
using System;
using System.Linq;

namespace STVRogue.GameLogic
{
    [TestFixture]
    /*Bugs found: 12
     All from TestConstructor, no bugs in the other methods.
     100% Dungeon (and Bridge and Node (excluding fight)) block coverage.*/
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
                        lvl = a - b - 1;
                    }
                    return p.isValidDungeon(d.startNode, d.exitNode, lvl)
                    .When(br != null);
                }
            ).QuickCheckThrowOnFailure();
        }

        [Test]
        /*Removing the a bridge of a lower level means the higher
         bridge will only be reachable from the end and not from the start.
         It's the other way around for removing a bridge of a higher level.
         This method tests both of these properties to verify the given levels are correct.*/
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

        [Test]
        /*Node level should always be 0, Dungeon size is irrelevant so no FsCheck.*/
        public void TestNodeLevel()
        {
            var d = new Dungeon(1, 1);
            Assert.Zero(d.level(new Node()));
        }

        [Test]
        /*Create a random dungeon and choose a random path from it.*/
        public void TestPath()
        {
            Prop.ForAll<uint, uint[], uint[]>(
                /*n is dungeon size, xs is random path from start, ys is random path from end
                 Intended to simulate selecting random nodes, but because shorter lists
                 are most likely generated more often, it's not completely random. 
                 But that's not important for this test.*/
                (n, xs, ys) =>
                {
                    if (n == 0)
                        return false.When(false);
                    var d = new Dungeon(n, 1);
                    Node a = d.startNode;
                    foreach (int x in xs)
                        a = a.neighbors[x % a.neighbors.Count];
                    Node b = d.exitNode;
                    foreach (int y in ys)
                        b = b.neighbors[y % b.neighbors.Count];
                    return p.isPath(Dungeon.shortestpath(a, b)).ToProperty();
                }
            ).QuickCheckThrowOnFailure();
        }

        [Test]
        /*Because only block coverage is needed for this project, testing if
         it fails for more complex graphs with disconnected nodes isn't necessary.
         Just testing if it can fail at all is enough.*/
        public void FailPath()
        {
            Assert.IsNull(Dungeon.shortestpath(new Node(), new Node()));
        }

        [Test]
        /*As far as I know, testing if a given path is the shortest is as
         complex as finding the path in the first place. Meaning that making a
         test in that way would be meaningless, considering that test containing
         an error is just as likely. This means I can't use FsCheck, and will use
         a regular unit test instead.*/
        public void TestShortest()
        {
            Node[] nodes = new Node[5];
            for (int i = 0; i < 5; i++)
                nodes[i] = new Node(i.ToString());
            nodes[0].connect(nodes[1]);
            nodes[0].connect(nodes[2]);
            nodes[1].connect(nodes[2]);
            nodes[1].connect(nodes[3]);
            nodes[2].connect(nodes[3]);
            nodes[2].connect(nodes[4]);
            nodes[3].connect(nodes[4]);
            Assert.AreEqual(
            Dungeon.shortestpath(nodes[0], nodes[4]).Select(nd => nd.id).ToArray(),
            new string[] { "0", "2", "4" });
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
