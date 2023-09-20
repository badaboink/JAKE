using JAKE.classlibrary;
using Microsoft.AspNetCore.SignalR;
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

        private List<Player> players = new List<Player>();
        private List<Obstacle> obstacles = new List<Obstacle>();
        private List<Enemy> enemies = new List<Enemy>();
        private int enemySpawnIntervalInSeconds = 10;
        private int enemyUpdateIntervalInMilliseconds = 200;

        public async Task SendColor(string color)
        {
            Player newplayer = new Player();
            lock (players)
            {
                int playerId = players.Count + 1;
                newplayer = new Player(playerId, "a", color);
                players.Add(newplayer);
                if (playerId == 1)
                {
                    obstacles = GameFunctions.GenerateObstacles();
                }
            }
            string connectionId = Context.ConnectionId;
            await Clients.Client(connectionId).SendAsync("YourPlayerInfo", newplayer.GetId(), newplayer.GetName(), newplayer.GetColor());
            Console.WriteLine("Client information sent:");
            //await Clients.All.SendAsync("PlayerList", players);

            //await Clients.Caller.SendAsync("YourPlayerInfo", newplayer.GetId(), newplayer.GetName(), newplayer.GetColor());
            //string obstacleData = string.Join(",", obstacles.Select(obstacle => obstacle.ToString()));
            //await Clients.Caller.SendAsync("ObstacleInfo", obstacleData);
            Console.WriteLine(newplayer.ToString());
            //Console.WriteLine(obstacleData);
        }

    }
}
