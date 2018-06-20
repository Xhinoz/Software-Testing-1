﻿using STVRogue.GameLogic;
using System;

namespace STVRogue.GameLogic
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

    public class Future : Specification
    {
        private Predicate<Game> p, q;
        private bool pre, post = false;

        public Future(Predicate<Game> p, Predicate<Game> q)
        {
            this.p = p;
            this.q = q;
        }

        /*Only returns false if it's the last turn and p has occurred but q hasn't.
         Always returns true otherwise.*/
        public bool test(Game g)
        {
            pre = pre || p(g);
            post = post || q(g);
            return !g.lastTurn || !pre || post;
        }
    }

    public class Implies : Specification
    {
        private Specification s, t;
        private bool pre, post = true;

        public Implies(Specification s, Specification t)
        {
            this.s = s;
            this.t = t;
        }

        /*Once again, can't be sure it's false until the last turn.
         Always returns true before then.*/
        public bool test(Game g)
        {
            pre = pre && s.test(g);
            post = post && s.test(g);
            return !g.lastTurn || !pre || post;
        }

        public bool relevant()
        {
            return pre;
        }
    }
}
