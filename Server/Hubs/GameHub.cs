using JAKE.classlibrary;
using Microsoft.AspNetCore.SignalR;
using Server.GameData;
using System.Drawing;
using System.Net.Sockets;
using System.Timers;
using System.Numerics;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Server.Hubs
{
    public class GameHub : Hub
    {
        private static readonly Dictionary<string, string> ConnectedClients = new Dictionary<string, string>();
        private readonly IGameDataService _gameDataService;
        public override async Task OnConnectedAsync()
        {
            string connectionId = Context.ConnectionId;
            Console.WriteLine(connectionId);
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            string connectionId = Context.ConnectionId;
            Console.WriteLine($"Disconnected: {connectionId}");
        }

        public bool IsClientConnected(string userId)
        {
            return ConnectedClients.ContainsKey(userId);
        }
        public GameHub(IGameDataService gameDataService)
        {
            _gameDataService = gameDataService;
        }

        //private async Task SendSpawnedEnemyAsync()
        //{
        //    Enemy enemy = _gameDataService.AddEnemies();
        //    Console.WriteLine($"{_gameDataService.GetEnemies().Count} {enemy.ToString()}");
        //    await Clients.All.SendAsync("SpawnedEnemy", enemy.ToString());
        //    Console.WriteLine("Timer restarted.");
        //}

        public async Task SendColor(string color)
        {
            Player newPlayer = _gameDataService.AddPlayer("a", color);
            try
            {
                Console.WriteLine($"Player {newPlayer.ToString()}");
                await Clients.Caller.SendAsync("YourPlayerInfo", newPlayer.GetId(), newPlayer.GetName(), newPlayer.GetColor());

                Console.WriteLine(_gameDataService.GetObstacleData());
                await Clients.Caller.SendAsync("ObstacleInfo", _gameDataService.GetObstacleData());

                await Clients.All.SendAsync("PlayerList", _gameDataService.GetPlayerList());

                // send existing enemy list
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public async Task SendMove(int id, double x, double y)
        {
            Console.WriteLine("Sending move");
            _gameDataService.EditPlayerPosition(id-1, x, y);
            Console.WriteLine($"Move Player: {_gameDataService.GetPlayerData(id - 1)}");
            await Clients.Others.SendAsync("UpdateUsers", _gameDataService.GetPlayerData(id - 1));
        }
    }
}
