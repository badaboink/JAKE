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
            Console.WriteLine("Client information sent:");
            await Clients.Caller.SendAsync("YourPlayerInfo", newplayer.GetId(), newplayer.GetName(), newplayer.GetColor());
            string obstacleData = string.Join(",", obstacles.Select(obstacle => obstacle.ToString()));
            await Clients.Caller.SendAsync("ObstacleInfo", obstacleData);
            Console.WriteLine(newplayer.ToString());
            Console.WriteLine(obstacleData);
        }

    }
}
