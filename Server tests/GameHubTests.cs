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

namespace Server_tests
{
    public class GameHubTests
    {
        [Fact]
        public async void OnConnectedAsync_AddsObserverToDataService()
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
            var gameDataServiceField = typeof(GameHub).GetField("_gameDataService", BindingFlags.Instance | BindingFlags.NonPublic);
            if (gameDataServiceField != null)
            {
                var gameDataService = (IGameDataService)gameDataServiceField.GetValue(gameHub);

                await gameHub.OnConnectedAsync();

                Dictionary <string,Observer> observers = gameDataService.GetObservers();
                string firstKey = observers.FirstOrDefault().Key;
                Assert.NotNull(observers);
                Assert.Equal(connectionId, firstKey);

            }
            else
            {
                // Handle the case where the field is not found (e.g., throw an exception)
            }
        }
    }
}
