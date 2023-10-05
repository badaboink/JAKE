using JAKE.classlibrary;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Protocol;
using Server.GameData;
using System;
using System.Drawing;
using System.Net.Sockets;
using System.Numerics;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Server.Hubs
{
    public class GameHub : Hub
    {
        private static readonly Dictionary<string, string> ConnectedClients = new Dictionary<string, string>();
        private readonly IGameDataService _gameDataService;
        private Timer gameTimer;
        public override async Task OnConnectedAsync()
        {
            string connectionId = Context.ConnectionId;
            Console.WriteLine(connectionId);
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            string connectionId = Context.ConnectionId;
            Console.WriteLine($"Disconnected: {connectionId}");
            await base.OnDisconnectedAsync(exception);
        }

        public bool IsClientConnected(string userId)
        {
            return ConnectedClients.ContainsKey(userId);
        }
        public GameHub(IGameDataService gameDataService)
        {
            _gameDataService = gameDataService;
        }
        private object syncLock = new object();
        private Random random = new Random();
        public async Task SendEnemies()
        {
            lock(syncLock)
            {
                DateTime startTime = _gameDataService.GetCurrentGameTime();
                DateTime currentTime = DateTime.Now;
                TimeSpan elapsedTime = currentTime - startTime;
                Console.WriteLine($"Sending enemy update {DateTime.Now}");
                _gameDataService.UpdateEnemyPositions();
                if (elapsedTime.TotalSeconds >= 10 && _gameDataService.GetEnemies().Count <= 10)
                {
                    _gameDataService.AddEnemies();
                    Console.WriteLine($"Spawning enemy {startTime} - {DateTime.Now}. {_gameDataService.GetEnemies().Count}");
                    _gameDataService.SetGameTime(DateTime.Now);
                }
            }
            if (_gameDataService.GetEnemies().Count > 0)
            {
                Console.WriteLine($"Sending Enemies {Context.ConnectionId}");
                await Clients.Caller.SendAsync("SendingEnemies", _gameDataService.GetEnemies());
            }
        }
        public async Task SendColor(string color, string name)
        {
            Player newPlayer = _gameDataService.AddPlayer(name, color);
            try
            {
                Console.WriteLine($"Player {newPlayer.ToString()}");
                await Clients.Caller.SendAsync("YourPlayerInfo", newPlayer.GetId(), newPlayer.GetName(), newPlayer.GetColor());

                Console.WriteLine(_gameDataService.GetObstacleData());
                await Clients.Caller.SendAsync("ObstacleInfo", _gameDataService.GetObstacleData());

                await Clients.All.SendAsync("PlayerList", _gameDataService.GetPlayerList());

                await Clients.All.SendAsync("GameTime", _gameDataService.GetCurrentGameTime());

                if(_gameDataService.GetEnemies().Count != 0)
                {
                    await Clients.Caller.SendAsync("SpawnedEnemies", _gameDataService.GetEnemies());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public async Task SendMove(int id, double x, double y)
        {
            //Console.WriteLine("Sending move");
            _gameDataService.EditPlayerPosition(id-1, x, y);
            //Console.WriteLine($"Move Player: {_gameDataService.GetPlayerData(id - 1)}");
            await Clients.Others.SendAsync("UpdateUsers", _gameDataService.GetPlayerData(id - 1));
        }
        public async Task SendEnemyUpdate(string enemy)
        {
            string[] parts = enemy.Split(':');
            if (parts.Length == 5)
            {
                int id = int.Parse(parts[0]);
                string color = parts[1];
                int health = int.Parse(parts[4]);
                _gameDataService.UpdateEnemy(id, health);
                await Clients.Others.SendAsync("UpdateEnemyHealth", id, color, health);
            }
        }
        public async Task SendDeadEnemy(string enemy)
        {
            string[] parts = enemy.Split(':');
            if (parts.Length == 5)
            {
                int id = int.Parse(parts[0]);
                string color = parts[1];
                _gameDataService.RemoveEnemy(id);
                await Clients.Others.SendAsync("UpdateDeadEnemy", id, color);
            }
        }
    }
}
