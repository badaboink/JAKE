using JAKE.classlibrary;
using JAKE.classlibrary.Patterns;
using Server.GameData;
using System.Diagnostics;

namespace Server_tests
{
    public class InMemoryGameDataServiceTests
    {
        private readonly InMemoryGameDataService gameDataService;
        public InMemoryGameDataServiceTests()
        {
            gameDataService = new InMemoryGameDataService();
        }

        [Fact]
        public void Test_Add_Player()
        {
            string playerName = "name";
            string playerColor = "color";
            string connectionID = "connectionID";
            string shotColor = "red";
            string shotShape = "round";
            Player player1 = new Player(1, playerName, playerColor, shotColor, shotShape);
            player1.SetConnectionId(connectionID);
            Player player2 = gameDataService.AddPlayer(playerName, playerColor, shotColor, shotShape, connectionID);
            Assert.Equal(player1.GetId(), player2.GetId());
            Assert.Equal(player1.GetName(), player2.GetName());
            Assert.Equal(player1.GetColor(), player2.GetColor());
            Assert.Equal(player1.GetConnectionId(), player2.GetConnectionId());
        }

        [Fact]
        public void Test_Remove_Player()
        {
            string playerName = "name";
            string playerColor = "color";
            string connectionID = "connectionID";
            string shotColor = "red";
            string shotShape = "round";
            Player player1 = gameDataService.AddPlayer(playerName, playerColor, shotColor, shotShape, connectionID);
            Player player2 = gameDataService.RemovePlayer(player1.GetConnectionId());
            Assert.Equal(player1.GetId(), player2.GetId());
            Assert.Equal(player1.GetName(), player2.GetName());
            Assert.Equal(player1.GetColor(), player2.GetColor());
            Assert.Equal(player1.GetConnectionId(), player2.GetConnectionId());
        }

        [Fact]
        public void Test_Edit_Player_Position()
        {
            string playerName = "name";
            string playerColor = "color";
            string connectionID = "connectionID";
            string shotColor = "red";
            string shotShape = "round";
            Player player1 = new Player(1, playerName, playerColor, shotColor, shotShape);
            player1.SetConnectionId(connectionID);
            player1.SetCurrentPosition(42, 69);
            Player player2 = gameDataService.AddPlayer(playerName, playerColor, shotColor, shotShape, connectionID);
            gameDataService.EditPlayerPosition(0, 42, 69);
            Assert.Equal(player1.GetCurrentX(), player2.GetCurrentX());
            Assert.Equal(player1.GetCurrentY(), player2.GetCurrentY());
        }

        [Fact]
        public void Test_Get_Player_Data()
        {
            string playerName = "name";
            string playerColor = "color";
            string connectionID = "connectionID";
            string shotColor = "red";
            string shotShape = "round";
            Player player1 = new Player(1, playerName, playerColor, shotColor, shotShape);
            player1.SetConnectionId(connectionID);
            gameDataService.AddPlayer(playerName, playerColor, shotColor, shotShape, connectionID);
            Assert.Equal(player1.ToString(), gameDataService.GetPlayerData(0));
        }

        [Fact]
        public void Test_Add_100_Enemies()
        {
            for (int i = 0; i < 100; i++)
            {
                Assert.NotNull(gameDataService.AddEnemies());
            }
        }

        [Fact]
        public void Test_Get_Player_List()
         {
             string playerName1 = "name1";
             string playerName2 = "name2";
             string playerName3 = "name3";
             string playerColor1 = "color1";
             string playerColor2 = "color2";
             string playerColor3 = "color3";
             string connectionID1 = "connectionID1";
             string connectionID2 = "connectionID2";
             string connectionID3 = "connectionID3";
             string shotColor = "red";
             string shotShape = "round";
             Player player1 = new Player(1, playerName1, playerColor1, shotColor, shotShape);
             Player player2 = new Player(2, playerName2, playerColor2, shotColor, shotShape);
             Player player3 = new Player(3, playerName3, playerColor3, shotColor, shotShape);
             player1.SetConnectionId(connectionID1);
             player2.SetConnectionId(connectionID2);
             player3.SetConnectionId(connectionID3);
             List<string> list = new List<string>();
             list.Add(player1.ToString());
             list.Add(player2.ToString());
             list.Add(player3.ToString());
             gameDataService.AddPlayer(playerName1, playerColor1, shotColor, shotShape, connectionID1);
             gameDataService.AddPlayer(playerName2, playerColor2, shotColor, shotShape, connectionID2);
             gameDataService.AddPlayer(playerName3, playerColor3, shotColor, shotShape, connectionID3);

             Assert.Equal(list[0], gameDataService.GetPlayerList()[0]);
             Assert.Equal(list[1], gameDataService.GetPlayerList()[1]);
             Assert.Equal(list[2], gameDataService.GetPlayerList()[2]);
        }
       
    }
}
