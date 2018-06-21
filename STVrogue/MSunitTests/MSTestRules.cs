using Microsoft.VisualStudio.TestTools.UnitTesting;
using STVRogue.Utils;
using System;
using System.Linq;

namespace STVRogue.GameLogic
{
    public class MSTestRules
    {
        [TestMethod]
        public void RZone()
        {
            testAlways(pZone);
        }

        [TestMethod]
        public void RNode()
        {
            testAlways(pNode);
        }

        [TestMethod]
        public void RAlert()
        {
            testAlways(pAlert);
        }

        [TestMethod]
        public void REndZone()
        {
            testAlways(pEndZone);
        }

        [TestMethod]
        public void RS1()
        {
            testImplies(makePS1, _ => qS1);
        }

        [TestMethod]
        public void RS2()
        {
            testImplies(_ => pS2, makeQS2);
        }

        private void testAlways(Predicate<Game> p)
        {
            GamePlay[] runs = loadRuns();
            foreach (var run in runs)
                Assert.IsTrue(run.Replay(new Always(p)));
        }

        /*If at some point p has happened, then at some point q has happened.
        Closures are needed for some of the predicates, so different function type requested.*/
        private void testImplies(Func<Game, Predicate<Game>> p, Func<Game, Predicate<Game>> q)
        {
            GamePlay[] runs = loadRuns();
            int i = 0;
            foreach (var run in runs)
            {
                var s = new Implies(new Future(_ => true, p(run.getState())), new Future(_ => true, q(run.getState())));
                bool value = run.Replay(s);
                if (!s.relevant)
                    Console.WriteLine("Test " + i + " is irrelevant.");
                Assert.IsTrue(value);
                i++;
            }
        }

        private bool pZone(Game g)
        {
            foreach (Pack p in g.monsterPacks)
                if (p.level != g.dungeon.nodeLevel(p.location))
                    return false;
            return true;
        }

        private bool pNode(Game g)
        {
            foreach (Node n in g.dungeon.containedNodes())
                if (g.dungeon.capacity(n) < 0)
                    return false;
            return true;
        }

        private bool pAlert(Game g)
        {
            foreach (Pack p in g.monsterPacks)
                if (p.fled)
                   p.fled = false;
                else if (Dungeon.alert == p.level && p.prevLoc != null && p.location != p.prevLoc &&
                    distance(p.location, g.player) > distance(p.prevLoc, g.player))
                        return false;
            return true;
        }

        private bool pEndZone(Game g)
        {
            uint finalLvl = g.dungeon.difficultyLevel + 1;
            foreach (Pack p in g.monsterPacks)
                if (p.fled)
                    p.fled = false;
                else if (p.level == finalLvl && g.player.level == finalLvl && p.prevLoc != null)
                {
                    if (p.prevLoc == p.location)
                        foreach (Node n in p.location.neighbors)
                        {
                            if (g.dungeon.capacity(n) > p.members.Count &&
                                distance(n, g.player) < distance(p.location, g.player))
                                return false;
                        }
                    else if (distance(p.location, g.player) >= distance(p.prevLoc, g.player))
                        return false;
                }
            return true;
        }

        //Closure to keep track of dungeon size.
        private Predicate<Game> makePS1(Game a)
        {
            int prevSize = a.dungeon.containedNodes().Count;
            return g =>
            {
                int curSize = g.dungeon.containedNodes().Count;
                bool result = curSize < prevSize;
                prevSize = curSize;
                return result;
            };
        }

        private bool qS1(Game g)
        {
            return g.player.crystalUsed;
        }

        private bool pS2(Game g)
        {
            return g.player.location == g.dungeon.exitNode;
        }

        //Closure to keep track of visited bridges.
        private Predicate<Game> makeQS2(Game a)
        {
            bool[] bridges = new bool[a.dungeon.difficultyLevel];
            return g =>
            {
                if (g.player.location is Bridge b)
                    bridges[g.dungeon.level(b) - 1] = true;
                return bridges.All(x => x);
            };
        }

        private int distance(Node u, Player p)
        {
            return Dungeon.shortestpath(u, p.location).Count;
        }

        private GamePlay[] loadRuns()
        {
            throw new NotImplementedException();
        }
    }
}
