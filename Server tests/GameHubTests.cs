using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.SignalR;
using Moq;
using Server.GameData;
using Server.Hubs;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using System.Reflection;
using JAKE.classlibrary;
using Microsoft.AspNetCore.Hosting.Server;
using JAKE.classlibrary.Collectibles;
using JAKE.classlibrary.Enemies;

namespace Server_tests
{
    public class TestFixture : IDisposable
    {
        public GameHub gameHub { get; private set; }

        public TestFixture()
        {
            InitializeGameHub();
        }

        private void InitializeGameHub()
        {
            Mock<IHubCallerClients> mockClients = new Mock<IHubCallerClients>();
            Mock<IClientProxy> mockClientProxy = new Mock<IClientProxy>();
            mockClients.Setup(clients => clients.All).Returns(mockClientProxy.Object);

            IGameDataService memory = new InMemoryGameDataService();
            var connectionId = "mock_connection_id";
            var hubCallerContext = new Mock<HubCallerContext>();
            hubCallerContext.SetupGet(c => c.ConnectionId).Returns(connectionId);

            gameHub = new GameHub(memory)
            {
                Clients = mockClients.Object,
                Context = hubCallerContext.Object
            };
        }
        public void ResetGameData()
        {
            IGameDataService memory = new InMemoryGameDataService();
            // Update the game data service in the game hub
            var field = typeof(GameHub).GetField("_gameDataService", BindingFlags.Instance | BindingFlags.NonPublic);
            field.SetValue(gameHub, memory);
        }

        public void Dispose()
        {
            gameHub.Dispose();
        }
    }
    [Collection("Test Collection")]
    public class GameHubTests : IClassFixture<TestFixture>
    {
        private readonly TestFixture fixture;
        public GameHubTests(TestFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public async void OnConnectedAsync_AddsObserverToDataService()
        {
            GameHub gameHub = fixture.gameHub;

            var gameDataServiceField = typeof(GameHub).GetField("_gameDataService", BindingFlags.Instance | BindingFlags.NonPublic);
            var gameDataService = (IGameDataService)gameDataServiceField.GetValue(gameHub);

            await gameHub.OnConnectedAsync();

            Dictionary<string, Observer> observers = gameDataService.GetObservers();
            string firstKey = observers.FirstOrDefault().Key;
            Assert.NotNull(observers);
            Assert.Equal("mock_connection_id", firstKey);
            fixture.ResetGameData();
        }
        [Fact]
        public async void OnDisconnectedAsync_RemovesObserver()
        {
            GameHub gameHub = fixture.gameHub;

            var gameDataServiceField = typeof(GameHub).GetField("_gameDataService", BindingFlags.Instance | BindingFlags.NonPublic);
            var gameDataService = (IGameDataService)gameDataServiceField.GetValue(gameHub);
            await gameHub.OnConnectedAsync();
            await gameHub.SendColor("TestColor", "TestPlayer", "TestShotColor", "TestShotShape");

            Mock<IClientProxy> mockClientProxy1 = new Mock<IClientProxy>();
            var observer1 = new Observer(mockClientProxy1.Object);
            gameDataService.AddObserver("connection_id_1", observer1);

            Dictionary<string, Observer> observers = gameDataService.GetObservers();
            int observerscountfirst = observers.Count();

            Exception testException = null;
            await gameHub.OnDisconnectedAsync(testException);
            int observerscountsecond = gameDataService.GetObservers().Count();
            Assert.NotEqual(observerscountfirst, observerscountsecond);
            fixture.ResetGameData();
        }
        [Fact]
        public async Task SendColor_AddsPlayer()
        {
            GameHub gameHub = fixture.gameHub;
            await gameHub.OnConnectedAsync();
            var gameDataServiceField = typeof(GameHub).GetField("_gameDataService", BindingFlags.Instance | BindingFlags.NonPublic);
            var gameDataService = (IGameDataService)gameDataServiceField.GetValue(gameHub);
            Player expectedPlayer = new Player(1, "TestPlayer", "TestColor", "TestShotColor", "TestShotShape");
            await gameHub.SendColor(expectedPlayer.GetColor(), expectedPlayer.GetName(), expectedPlayer.GetShotColor(), expectedPlayer.GetShotShape());

            string resultPlayerString = gameDataService.GetPlayerList()[0].ToString();
            int indexOfFirstColon = resultPlayerString.IndexOf(':');
            Assert.Equal(expectedPlayer.ToString().Substring(2), resultPlayerString.Substring(indexOfFirstColon + 1));

            fixture.ResetGameData();
        }
        [Fact]
        public async Task SendEnemies_AddsEnemiesAndObjects()
        {
            GameHub gameHub = fixture.gameHub;
            await gameHub.OnConnectedAsync();
            await gameHub.SendColor("TestColor", "TestPlayer", "TestShotColor", "TestShotShape");
            var gameDataServiceField = typeof(GameHub).GetField("_gameDataService", BindingFlags.Instance | BindingFlags.NonPublic);
            var gameDataService = (IGameDataService)gameDataServiceField.GetValue(gameHub);
            gameDataService.SetGameTime(DateTime.Now.AddSeconds(-10));
            await gameHub.SendEnemies();
            Assert.NotEmpty(gameDataService.GetEnemies());
            Assert.NotEmpty(gameDataService.GetCoins());
            Assert.NotEmpty(gameDataService.GetShields());
            Assert.NotEmpty(gameDataService.GetHealthBoosts());
            Assert.NotEmpty(gameDataService.GetSpeedBoosts());
            fixture.ResetGameData();
        }
        [Fact]
        public async Task SendPickedCoin_RemovesCoin()
        {
            GameHub gameHub = fixture.gameHub;
            var gameDataServiceField = typeof(GameHub).GetField("_gameDataService", BindingFlags.Instance | BindingFlags.NonPublic);
            var gameDataService = (IGameDataService)gameDataServiceField.GetValue(gameHub);

            Coin coin = gameDataService.AddCoin(2);
            coin.Image = "";

            string coinString = coin.ToXML();

            Mock<IClientProxy> mockClientProxy = new Mock<IClientProxy>();
            var observer = new Observer(mockClientProxy.Object);
            gameDataService.AddObserver("mock_connection_id", observer);

            await gameHub.SendPickedCoin(coinString);

            Assert.Empty(gameDataService.GetCoins());
            fixture.ResetGameData();
        }
        [Fact]
        public async Task SendPickedShield_RemovesShield()
        {
            GameHub gameHub = fixture.gameHub;
            var gameDataServiceField = typeof(GameHub).GetField("_gameDataService", BindingFlags.Instance | BindingFlags.NonPublic);
            var gameDataService = (IGameDataService)gameDataServiceField.GetValue(gameHub);

            Shield shield = gameDataService.AddShield(5);

            Mock<IClientProxy> mockClientProxy = new Mock<IClientProxy>();
            var observer = new Observer(mockClientProxy.Object);
            gameDataService.AddObserver("mock_connection_id", observer);

            var observer2 = new Observer(mockClientProxy.Object);
            gameDataService.AddObserver("mock_connection_id2", observer2);

            await gameHub.SendPickedShield(shield.ToString());

            var shields = gameDataService.GetShields();
            Assert.DoesNotContain(shield.ToString(), shields);
            fixture.ResetGameData();
        }
        [Fact]
        public async Task SendPickedHealthBoost_RemovesHealthBoost()
        {
            GameHub gameHub = fixture.gameHub;
            var gameDataServiceField = typeof(GameHub).GetField("_gameDataService", BindingFlags.Instance | BindingFlags.NonPublic);
            var gameDataService = (IGameDataService)gameDataServiceField.GetValue(gameHub);

            HealthBoost hb = gameDataService.AddHealthBoost(42);

            Mock<IClientProxy> mockClientProxy = new Mock<IClientProxy>();
            var observer = new Observer(mockClientProxy.Object);
            gameDataService.AddObserver("mock_connection_id", observer);

            Mock<IClientProxy> otherObserverClientProxy = new Mock<IClientProxy>();
            var otherObserver = new Observer(otherObserverClientProxy.Object);
            gameDataService.AddObserver("other_connection_id", otherObserver);

            await gameHub.SendPickedHealthBoost(hb.ToString());

            var healthBoosts = gameDataService.GetHealthBoosts();
            Assert.DoesNotContain(hb.ToString(), healthBoosts);
            fixture.ResetGameData();
        }
        [Fact]
        public async Task SendPickedSpeedBoost_RemovesSpeedBoostAndNotifiesObserver()
        {
            GameHub gameHub = fixture.gameHub;
            var gameDataServiceField = typeof(GameHub).GetField("_gameDataService", BindingFlags.Instance | BindingFlags.NonPublic);
            var gameDataService = (IGameDataService)gameDataServiceField.GetValue(gameHub);

            SpeedBoost sb = gameDataService.AddSpeedBoost(1);

            Mock<IClientProxy> mockClientProxy = new Mock<IClientProxy>();
            var observer = new Observer(mockClientProxy.Object);
            gameDataService.AddObserver("mock_connection_id", observer);

            await gameHub.SendPickedSpeedBoost(sb.ToString());

            Assert.Empty(gameDataService.GetSpeedBoosts());
            fixture.ResetGameData();
        }
        [Fact]
        public async Task UpdateDeadPlayer_ChangesPlayer()
        {
            GameHub gameHub = fixture.gameHub;
            var gameDataServiceField = typeof(GameHub).GetField("_gameDataService", BindingFlags.Instance | BindingFlags.NonPublic);
            var gameDataService = (IGameDataService)gameDataServiceField.GetValue(gameHub);

            Player player1 = gameDataService.AddPlayer("name", "color", "mock_connection_id", "shotcolor", "shotshape");
            Player player2 = gameDataService.AddPlayer("name", "color", "mock_connection_id", "shotcolor", "shotshape");


            Mock<IClientProxy> mockClientProxy1 = new Mock<IClientProxy>();
            var observer1 = new Observer(mockClientProxy1.Object);
            gameDataService.AddObserver("connection_id_1", observer1);
            Mock<IClientProxy> mockClientProxy2 = new Mock<IClientProxy>();
            var observer2 = new Observer(mockClientProxy2.Object);
            gameDataService.AddObserver("connection_id_2", observer2);

            await gameHub.UpdateDeadPlayer(player1.GetId());
            
            Assert.Equal("DEAD", player1.GetName());
            Assert.Equal("Black", player1.GetColor());
            fixture.ResetGameData();
        }
        [Fact]
        public async Task SendMove_EditsPlayerPosition()
        {
            GameHub gameHub = fixture.gameHub;
            var gameDataServiceField = typeof(GameHub).GetField("_gameDataService", BindingFlags.Instance | BindingFlags.NonPublic);
            var gameDataService = (IGameDataService)gameDataServiceField.GetValue(gameHub);

            Player player1 = gameDataService.AddPlayer("name", "color", "connectionid", "shotcolor", "shotshape");

            double newX = 100.0;
            double newY = 200.0;

            Mock<IClientProxy> mockClientProxy1 = new Mock<IClientProxy>();
            var observer1 = new Observer(mockClientProxy1.Object);
            gameDataService.AddObserver("connection_id_1", observer1);

            await gameHub.SendMove(player1.GetId(), newX, newY);

            Assert.Equal(newX, player1.GetCurrentX());
            Assert.Equal(newY, player1.GetCurrentY());
            fixture.ResetGameData();
        }
        [Fact]
        public async Task SendDeadEnemy_NotifiesObservers()
        {
            GameHub gameHub = fixture.gameHub;
            var gameDataServiceField = typeof(GameHub).GetField("_gameDataService", BindingFlags.Instance | BindingFlags.NonPublic);
            var gameDataService = (IGameDataService)gameDataServiceField.GetValue(gameHub);

            Mock<IClientProxy> mockClientProxy1 = new Mock<IClientProxy>();
            var observer1 = new Observer(mockClientProxy1.Object);
            gameDataService.AddObserver("connection_id_1", observer1);

            Enemy enemy = gameDataService.AddEnemies();
            await gameHub.SendDeadEnemy(enemy.ToString());
            Assert.Empty(gameDataService.GetEnemies());
            fixture.ResetGameData();
        }
        [Fact]
        public async Task SendEnemyUpdate_UpdatesEnemyAndNotifiesObservers()
        {
            GameHub gameHub = fixture.gameHub;
            var gameDataServiceField = typeof(GameHub).GetField("_gameDataService", BindingFlags.Instance | BindingFlags.NonPublic);
            var gameDataService = (IGameDataService)gameDataServiceField.GetValue(gameHub);

            Enemy enemy = gameDataService.AddEnemies();
            Enemy changedEnemy = new Enemy(enemy.GetId(), enemy.GetColor());
            changedEnemy.SetHealth(1);
            int beforeChange = enemy.GetHealth();

            Mock<IClientProxy> mockClientProxy = new Mock<IClientProxy>();
            var observer = new Observer(mockClientProxy.Object);
            gameDataService.AddObserver("mock_connection_id", observer);

            await gameHub.SendEnemyUpdate(changedEnemy.ToString());
            int afterChange = enemy.GetHealth();

            Assert.NotEqual(beforeChange, afterChange);
            fixture.ResetGameData();
        }
    }
}
