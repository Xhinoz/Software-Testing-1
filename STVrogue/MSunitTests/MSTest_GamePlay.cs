using Microsoft.VisualStudio.TestTools.UnitTesting;
using STVRogue.Utils;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;


namespace STVRogue.GameLogic
{
    [TestClass]
    public class MSTest_GamePlay
    { 

        /*[TestMethod]
        public void MSTest_GamePlay_SerializeGame()
        {
            Game g = new Game(1, 1, 1);
            g.SerializeGame();
        }*/

        [TestMethod]
        public void MSTest_GamePlay_DeserializeGame()
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(@"D:\prog\Software-Testing-1\STVrogue\GamePlays\ExampleNew.txt", FileMode.Open, FileAccess.Read);
            Game objnew = (Game)formatter.Deserialize(stream);

            Debug.WriteLine(objnew.player.HP);
            Debug.WriteLine(objnew.monsterPacks.Count.ToString());
            
        }
    }
}
