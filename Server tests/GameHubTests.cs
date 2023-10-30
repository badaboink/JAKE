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

namespace Server_tests
{
    public class GameHubTests
    {
        private GameHub CreateGameHub()
        {
            Mock<IHubCallerClients> mockClients = new Mock<IHubCallerClients>();
            Mock<IClientProxy> mockClientProxy = new Mock<IClientProxy>();
            mockClients.Setup(clients => clients.All).Returns(mockClientProxy.Object);

            IGameDataService memory = new InMemoryGameDataService();
            var connectionId = "mock_connection_id";
            var hubCallerContext = new Mock<HubCallerContext>();
            hubCallerContext.SetupGet(c => c.ConnectionId).Returns(connectionId);

            GameHub gameHub = new GameHub(memory)
            {
                Clients = mockClients.Object,
                Context = hubCallerContext.Object
            };

            return gameHub;
        }

        [Fact]
        public async void OnConnectedAsync_AddsObserverToDataService()
        {
            // Create a GameHub instance using the setup method
            GameHub gameHub = CreateGameHub();

            var gameDataServiceField = typeof(GameHub).GetField("_gameDataService", BindingFlags.Instance | BindingFlags.NonPublic);
            var gameDataService = (IGameDataService)gameDataServiceField.GetValue(gameHub);

            await gameHub.OnConnectedAsync();

            Dictionary<string, Observer> observers = gameDataService.GetObservers();
            string firstKey = observers.FirstOrDefault().Key;
            Assert.NotNull(observers);
            Assert.Equal("mock_connection_id", firstKey);
        }
        [Fact]
        public async void OnDisconnectedAsync_RemovesObserver()
        {
            GameHub gameHub = CreateGameHub();

            var gameDataServiceField = typeof(GameHub).GetField("_gameDataService", BindingFlags.Instance | BindingFlags.NonPublic);
            var gameDataService = (IGameDataService)gameDataServiceField.GetValue(gameHub);

            await gameHub.OnConnectedAsync();
            await gameHub.SendColor("TestColor", "TestPlayer", "TestShotColor", "TestShotShape");

            Dictionary<string, Observer> observers = gameDataService.GetObservers();
            int observerscountfirst = observers.Count();

            Exception testException = null;
            await gameHub.OnDisconnectedAsync(testException);
            int observerscountsecond = gameDataService.GetObservers().Count();
            Assert.NotEqual(observerscountfirst, observerscountsecond);
        }
        [Fact]
        public async Task SendColor_AddsPlayerAndSendsGameStart()
        {
            GameHub gameHub = CreateGameHub();
            await gameHub.OnConnectedAsync();

            Player expectedPlayer = new Player(1, "TestPlayer", "TestColor", "TestShotColor", "TestShotShape");
            await gameHub.SendColor(expectedPlayer.GetColor(), expectedPlayer.GetName(), expectedPlayer.GetShotColor(), expectedPlayer.GetShotShape());
            var gameDataServiceField = typeof(GameHub).GetField("_gameDataService", BindingFlags.Instance | BindingFlags.NonPublic);
            var gameDataService = (IGameDataService)gameDataServiceField.GetValue(gameHub);

            string resultPlayerString = gameDataService.GetPlayerList()[0].ToString();
            int indexOfFirstColon = resultPlayerString.IndexOf(':');
            Assert.Equal(expectedPlayer.ToString().Substring(2), resultPlayerString.Substring(indexOfFirstColon + 1));
        }

    }
}
