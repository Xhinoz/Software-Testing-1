using Microsoft.VisualStudio.TestTools.UnitTesting;
using STVRogue.Utils;
using System;
using System.Linq;

namespace STVRogue.GameLogic
{
    public class MSTestRules
    {
        private Predicates ps = new Predicates();

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

        private void testAlways(Predicate<Game> p)
        {
            GamePlay[] runs = loadRuns();
            foreach (var run in runs)
                Assert.IsTrue(run.Replay(new Always(p)));
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
            foreach (Node n in ps.reachableNodes(g.dungeon.startNode))
                if (n.packs.Sum(p => p.members.Count) > g.dungeon.capacity(n))
                    return false;
            return true;
        }



        private GamePlay[] loadRuns()
        {
            throw new NotImplementedException();
        }
    }
}
