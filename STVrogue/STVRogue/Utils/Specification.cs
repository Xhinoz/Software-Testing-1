using STVRogue.GameLogic;
using System;

namespace STVRogue.Utils
{
    public interface Specification
    {
        bool test(Game g);
    }

    public class Always : Specification
    {
        private Predicate<Game> p;

        public Always(Predicate<Game> p) { this.p = p; }

        public bool test(Game g) { return p(g); }
    }

    public class Unless : Specification
    {
        private Predicate<Game> p, q;
        private bool previous = false;

        public Unless(Predicate<Game> p, Predicate<Game> q)
        {
            this.p = p;
            this.q = q;
        }

        public bool test(Game g)
        {
            bool verdict = !previous || p(g) || q(g);
            previous = p(g) && !q(g);
            return verdict;
        }
    }
}
