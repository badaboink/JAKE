using JAKE.classlibrary;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Server.GameData
{
    [ExcludeFromCodeCoverage]
    public class Observer
    {
        private readonly IClientProxy clientProxy;

        public Observer(IClientProxy clientProxy)
        {
            this.clientProxy = clientProxy;
        }
        public async Task GameUpdate(List<string> playerlist, DateTime gametime)
        {
            if (clientProxy != null)
            {
                await clientProxy.SendAsync("GameUpdate", playerlist, gametime);
            }
        }
        public async Task HandleMoveUpdate(string player)
        {
            if (clientProxy != null)
            {
                await clientProxy.SendAsync("UpdateUsers", player);
            }
        }

        public async Task HandleEnemyUpdate(int id, string color, int health)
        {
            if (clientProxy != null)
            {
                await clientProxy.SendAsync("UpdateEnemyHealth", id, color, health);
            }
        }

        public async Task HandleShotFired(int player_id, double directionX, double directionY)
        {
            await clientProxy.SendAsync("UpdateShotsFired", player_id, directionX, directionY);
        }

        public async Task HandleDeadEnemy(int id, string color)
        {
            if (clientProxy != null)
            {
                await clientProxy.SendAsync("UpdateDeadEnemy", id, color);
            }
        }
        public async Task GameStart(Player newPlayer, string obstacles)
        {
            if(clientProxy!=null)
            {
                await clientProxy.SendAsync("GameStart", newPlayer.GetId(), newPlayer.GetName(), newPlayer.GetColor(), obstacles);
            }
        }
        public async Task HandleEnemies(List<string> enemies)
        {
            if (clientProxy != null)
            {
                await clientProxy.SendAsync("SendingEnemies", enemies);
            }
        }
        public async Task HandleDisconnectedPlayer(string player)
        {
            if (clientProxy != null)
            {
                await clientProxy.SendAsync("DisconnectedPlayer", player);
            }
        }

        public async Task HandleCoins(List<string> coins)
        {
            if (clientProxy != null)
            {
                await clientProxy.SendAsync("SendingCoins", coins);
            }
        }
        public async Task HandlePickedCoin(string id)
        {
            if (clientProxy != null)
            {
                await clientProxy.SendAsync("SendingPickedCoin", id);
            }
        }
        public async Task HandleShields(List<string> shields)
        {
            if (clientProxy != null)
            {
                await clientProxy.SendAsync("SendingShields", shields);
            }
        }
        public async Task HandlePickedShield(int id)
        {
            if (clientProxy != null)
            {
                await clientProxy.SendAsync("SendingPickedShield", id);
            }
        }

        public async Task HandleHealthBoosts(List<string> healthBoosts)
        {
            if (clientProxy != null)
            {
                await clientProxy.SendAsync("SendingHealthBoosts", healthBoosts);
            }
        }
        public async Task HandlePickedHealthBoost(int id)
        {
            if (clientProxy != null)
            {
                await clientProxy.SendAsync("SendingPickedHealthBoost", id);
            }
        }

        public async Task HandleSpeedBoosts(List<string> speedBoosts)
        {
            if (clientProxy != null)
            {
                await clientProxy.SendAsync("SendingSpeedBoosts", speedBoosts);
            }
        }
        public async Task HandlePickedSpeedBoost(int id)
        {
            if (clientProxy != null)
            {
                await clientProxy.SendAsync("SendingPickedSpeedBoost", id);
            }
        }

        public async Task SendPlayerMessage(string name, string message)
        {
            if(clientProxy != null)
            {
                await clientProxy.SendAsync("MessageSent", name, message);
            }
        }

    }
}
