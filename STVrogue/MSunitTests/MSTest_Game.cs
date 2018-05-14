using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using STVRogue.GameLogic;
using STVRogue.Utils;

namespace UnitTests_STVRogue
{
    [TestClass]
    public class MSTest_Game
    {
        [TestMethod]
        public void MSTest_create_random_game()
        {
            uint difficultyLevel = (uint)RandomGenerator.rnd.Next(100);
            uint nodeCapacityMultiplier = (uint)RandomGenerator.rnd.Next(5);
            uint numberOfMonsters = (uint)RandomGenerator.rnd.Next(100);

            for(int i = 0; i < 100; ++i)
            {
                Game game = new Game(difficultyLevel, nodeCapacityMultiplier, numberOfMonsters);
            
            }
        }

    }
}
