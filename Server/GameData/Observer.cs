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
    }
}
