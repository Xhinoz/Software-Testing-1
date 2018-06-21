using Microsoft.VisualStudio.TestTools.UnitTesting;
using STVRogue.Utils;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;


namespace STVRogue.GameLogic
{
    [TestClass]
    public class MSTest_GamePlay
    {
        public List<GamePlay> LoadSavedGamePlays(params string[] files)
        {
            List<GamePlay> gp = new List<GamePlay>();
            foreach(string s in files)
            {
                gp.Add(new GamePlay(s));
            }

            return gp;
        }

        [TestMethod]
        public void MSTest_GamePlay_ReplayGame()
        {
            List<GamePlay> gps = LoadSavedGamePlays("test.txt");
            foreach(GamePlay gp in gps)
            {
                Specification S = new HPBelow100_Spec();
                Assert.IsTrue(gp.Replay(S));
            }

            //GamePlay gp = new GamePlay("test.txt"); //test 1 gameplay
        }

        
    }
}
