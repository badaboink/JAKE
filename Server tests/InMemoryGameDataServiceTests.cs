using JAKE.classlibrary;
using JAKE.classlibrary.Collectibles;
using JAKE.classlibrary.Enemies;
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
            Player player2 = gameDataService.AddPlayer(playerName, playerColor, connectionID, shotColor, shotShape);
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
            Player player2 = gameDataService.AddPlayer(playerName, playerColor, connectionID, shotColor, shotShape);
            gameDataService.EditPlayerPosition(player2.GetId(), 42, 69);
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
            Player player2 = gameDataService.AddPlayer(playerName, playerColor, connectionID, shotColor, shotShape);
            string secondplayersstring = gameDataService.GetPlayerData(player2.GetId());
            int indexOfFirstColon = secondplayersstring.IndexOf(':');
            // we dont need to check the id as they are generated randomly
            Assert.Equal(player1.ToString().Substring(2), secondplayersstring.Substring(indexOfFirstColon + 1));
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
            gameDataService.AddPlayer(playerName1, playerColor1, connectionID1, shotColor, shotShape);
            gameDataService.AddPlayer(playerName2, playerColor2, connectionID2, shotColor, shotShape);
            gameDataService.AddPlayer(playerName3, playerColor3, connectionID3, shotColor, shotShape);

            string first = gameDataService.GetPlayerList()[0];
            int index1 = first.IndexOf(':')+1;
            string second = gameDataService.GetPlayerList()[1];
            int index2 = second.IndexOf(':') + 1;
            string third = gameDataService.GetPlayerList()[2];
            int index3 = third.IndexOf(':') + 1;

            Assert.Equal(list[0].Substring(2), first.Substring(index1));
            Assert.Equal(list[1].Substring(2), second.Substring(index2));
            Assert.Equal(list[2].Substring(2), third.Substring(index3));
        }

        [Fact]
        public void Test_Update_Enemy()
        { 
            Enemy enemy = gameDataService.AddEnemies();
            int id = enemy.GetId();
            gameDataService.UpdateEnemy(id, 50);
            int expected = 50;
            int actual = enemy.GetHealth();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Test_Remove_Enemy()
        {
            Enemy enemy = gameDataService.AddEnemies();
            int listCountBefore = gameDataService.GetEnemies().Count;
            int id = enemy.GetId();
            gameDataService.RemoveEnemy(id);
            int listCountAfter = gameDataService.GetEnemies().Count;
            int difference = listCountBefore - listCountAfter;

            Assert.Equal(1, difference);
        }
        
        [Fact]
        public void Test_Get_Enemies()
        {
            int actual = gameDataService.GetEnemies().Count;
            int expected = 1;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Test_Add_Coin()
        {
            Coin coin = gameDataService.AddCoin(10);
            int expected = 10;
            int actual = coin.Points;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Test_Return_Coin()
        {
            Coin coin = gameDataService.AddCoin(10);
            int id = coin.id;

            Coin returned = gameDataService.ReturnCoin(id);

            Assert.Equal(coin, returned);
        }

        [Fact]
        public void Test_Remove_Coin()
        {
            Coin coin = gameDataService.AddCoin(10);
            int listCountBefore = gameDataService.GetCoins().Count;
            int id = coin.id;
            gameDataService.RemoveCoin(id);
            int listCountAfter = gameDataService.GetCoins().Count;
            int difference = listCountBefore - listCountAfter;

            Assert.Equal(1, difference);
        }

        [Fact]
        public void Test_Get_Coins()
        {
            gameDataService.AddCoin(10);
            int actual = gameDataService.GetCoins().Count;
            int expected = 1;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Test_Add_HealthBoost()
        {
            HealthBoost healthBoost = gameDataService.AddHealthBoost(10);
            int expected = 10;
            int actual = healthBoost.Health;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Test_Remove_HealthBoost()
        {
            HealthBoost healthBoost = gameDataService.AddHealthBoost(10);
            int listCountBefore = gameDataService.GetHealthBoosts().Count;
            int id = healthBoost.id;
            gameDataService.RemoveHealthBoost(id);
            int listCountAfter = gameDataService.GetHealthBoosts().Count;
            int difference = listCountBefore - listCountAfter;

            Assert.Equal(1, difference);
        }

        [Fact]
        public void Test_Get_HealthBoost()
        {
            gameDataService.AddHealthBoost(10);
            int actual = gameDataService.GetHealthBoosts().Count;
            int expected = 1;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Test_Add_SpeedBoost()
        {
            SpeedBoost speedBoost = gameDataService.AddSpeedBoost(10);
            int expected = 10;
            int actual = speedBoost.Speed;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Test_Remove_SpeedBoost()
        {
            SpeedBoost speedBoost = gameDataService.AddSpeedBoost(10);
            int listCountBefore = gameDataService.GetSpeedBoosts().Count;
            int id = speedBoost.id;
            gameDataService.RemoveSpeedBoost(id);
            int listCountAfter = gameDataService.GetSpeedBoosts().Count;
            int difference = listCountBefore - listCountAfter;

            Assert.Equal(1, difference);
        }

        [Fact]
        public void Test_Get_SpeedBoost()
        {
            gameDataService.AddSpeedBoost(10);
            int actual = gameDataService.GetSpeedBoosts().Count;
            int expected = 1;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Test_Add_Shield()
        {
            Shield shield = gameDataService.AddShield(10);
            int expected = 10;
            int actual = shield.Time;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Test_Remove_Shield()
        {
            Shield shield = gameDataService.AddShield(10);
            int listCountBefore = gameDataService.GetShields().Count;
            int id = shield.id;
            gameDataService.RemoveShield(id);
            int listCountAfter = gameDataService.GetShields().Count;
            int difference = listCountBefore - listCountAfter;

            Assert.Equal(1, difference);
        }

        [Fact]
        public void Test_Get_Shield()
        {
            gameDataService.AddShield(10);
            int actual = gameDataService.GetShields().Count;
            int expected = 1;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Test_Update_Dead_Player()
        {

            Player player = gameDataService.AddPlayer("vardenis", "red", "conn123","red", "round");
            int id = player.GetId();
            gameDataService.UpdateDeadPlayer(id);
            string updated = gameDataService.GetPlayerData(id);
            string[] parts = updated.Split(':');
            string actualName = parts[1];
            string expected = "DEAD";

            Assert.Equal(expected, actualName);

        }

        [Fact]
        public void Test_SetGetGameTime()
        {
            DateTime datetime = DateTime.Now;
            gameDataService.SetGameTime(datetime);
            DateTime settedDatetime = gameDataService.GetCurrentGameTime();

            Assert.Equal(datetime, settedDatetime);
        }
       
    }
}
