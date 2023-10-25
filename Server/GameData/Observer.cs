using JAKE.classlibrary;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Server.GameData
{
    public class Observer
    {
        private readonly IClientProxy clientProxy;

        public Observer(IClientProxy clientProxy)
        {
            this.clientProxy = clientProxy;
        }
        public async Task GameUpdate(List<string> playerlist, DateTime gametime)
        {
            await clientProxy.SendAsync("GameUpdate", playerlist, gametime);
        }
        public async Task HandleMoveUpdate(string player)
        {
            await clientProxy.SendAsync("UpdateUsers", player);
        }

        public async Task HandleEnemyUpdate(int id, string color, int health)
        {
            await clientProxy.SendAsync("UpdateEnemyHealth", id, color, health);
        }

        public async Task HandleShotFired(int player_id, double directionX, double directionY)
        {
            await clientProxy.SendAsync("UpdateShotsFired", player_id, directionX, directionY);
        }

        public async Task HandleDeadEnemy(int id, string color)
        {
            await clientProxy.SendAsync("UpdateDeadEnemy", id, color);
        }
        public async Task GameStart(Player newPlayer, string obstacles)
        {
            await clientProxy.SendAsync("GameStart", newPlayer.GetId(), newPlayer.GetName(), newPlayer.GetColor(), obstacles);
        }
        public async Task HandleEnemies(List<string> enemies)
        {
            await clientProxy.SendAsync("SendingEnemies", enemies);
        }
        public async Task HandleDisconnectedPlayer(string player)
        {
            await clientProxy.SendAsync("DisconnectedPlayer", player);
        }

        public async Task HandleCoins(List<string> coins)
        {
            await clientProxy.SendAsync("SendingCoins", coins);
        }
        public async Task HandlePickedCoin(string id)
        {
            await clientProxy.SendAsync("SendingPickedCoin", id);
        }
        public async Task HandleShields(List<string> shields)
        {
            await clientProxy.SendAsync("SendingShields", shields);
        }
        public async Task HandlePickedShield(int id)
        {
            await clientProxy.SendAsync("SendingPickedShield", id);
        }

        public async Task HandleHealthBoosts(List<string> healthBoosts)
        {
            await clientProxy.SendAsync("SendingHealthBoosts", healthBoosts);
        }
        public async Task HandlePickedHealthBoost(int id)
        {
            await clientProxy.SendAsync("SendingPickedHealthBoost", id);
        }

        public async Task HandleSpeedBoosts(List<string> speedBoosts)
        {
            await clientProxy.SendAsync("SendingSpeedBoosts", speedBoosts);
        }
        public async Task HandlePickedSpeedBoost(int id)
        {
            await clientProxy.SendAsync("SendingPickedSpeedBoost", id);
        }

        public async Task HandleBossZombie(List<string> boss)
        {
            await clientProxy.SendAsync("SendingBossZombie", boss);
        }
        public async Task HandleDeadBossZombie(string name)
        {
            await clientProxy.SendAsync("SendingDeadBossZombie", name);
        }
        public async Task HandleDeadMiniZombie(string name)
        {
            await clientProxy.SendAsync("SendingDeadMiniZombie", name);
        }
    }
}
