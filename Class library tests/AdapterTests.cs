using JAKE.classlibrary;
using JAKE.classlibrary.Collectibles;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Class_library_tests
{
    public class AdapterTests
    {
        [Fact]
        public void Test_ConsoleString_Convert_ToServerString_Coin()
        {
            string expectedCoinString = "10:1:5:5:20:20:image";
            string consoleCoin = "points:10,id:1,x:5,y:5,width:20,height:20,image:image";
            string coinString = new ServerString(consoleCoin).ConvertedString;

            Assert.Equal(expectedCoinString, coinString);
        }

        [Fact]
        public void Test_JsonString_Convert_ToServerString_Coin()
        {
            //id x y points, width,height = 20
            Coin coin = new Coin(1, 5, 5, 10, "image");
            string expectedCoinString = "10:1:5:5:20:20:image";
            string jsonCoin = JsonConvert.SerializeObject(coin);
            string coinString = new ServerString(jsonCoin).ConvertedString;

            Assert.Equal(expectedCoinString, coinString);
        }

        [Fact]
        public void Test_XMLString_Convert_ToServerString_Coin()
        {
            //id x y points, width,height = 20
            Coin coin = new Coin(1, 5, 5, 10, "image");
            string expectedCoinString = "10:1:5:5:20:20:image";
            string xmlCoin = coin.ToXML();
            string coinString = new ServerString(xmlCoin).ConvertedString;

            Assert.Equal(expectedCoinString, coinString);
        }
    }
}
