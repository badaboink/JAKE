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

namespace Server_tests
{
    public class GameHubTests
    {
        [Fact]
        public async Task OnConnectedAsync_Should_AddObserverToGameData()
        {

        }
    }
}
