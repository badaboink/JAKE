using JAKE.classlibrary;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Protocol;
using Server.GameData;
using System;
using System.Drawing;
using System.IO;
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
        public override async Task OnConnectedAsync()
        {
            string connectionId = Context.ConnectionId;
            Observer observer = new Observer(Clients.Client(connectionId));
            _gameDataService.AddObserver(connectionId, observer);
            Console.WriteLine(connectionId);
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            string connectionId = Context.ConnectionId;
            _gameDataService.RemoveObserver(connectionId);
            Console.WriteLine($"Disconnected: {connectionId}");
            Player playertoremove = _gameDataService.RemovePlayer(connectionId);
            Console.WriteLine(_gameDataService.GetPlayerList().Count);
            Dictionary<string, Observer> observers = _gameDataService.GetObservers();
            foreach (var observerEntry in observers)
            {
                await observerEntry.Value.HandleDisconnectedPlayer(playertoremove.ToString());
            }
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
        
        //TODO: SendCoins
        public async Task SendColor(string color, string name)
        {
            try
            {
                Player newPlayer = _gameDataService.AddPlayer(name, color, Context.ConnectionId);
                Console.WriteLine($"Player {newPlayer.ToString()}");

                Dictionary<string, Observer> observers = _gameDataService.GetObservers();

                await observers[Context.ConnectionId].GameStart(newPlayer, _gameDataService.GetObstacleData());
                List<string> playerlist = _gameDataService.GetPlayerList();
                DateTime currentgametime = _gameDataService.GetCurrentGameTime();
                foreach (var observerEntry in observers)
                {
                    await observerEntry.Value.GameUpdate(playerlist, currentgametime);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public async Task SendEnemies()
        {
            lock (syncLock)
            {
                DateTime startTime = _gameDataService.GetCurrentGameTime();
                DateTime currentTime = DateTime.Now;
                TimeSpan elapsedTime = currentTime - startTime;
                //Console.WriteLine($"Sending enemy update {DateTime.Now}");
                
                _gameDataService.UpdateEnemyPositions();
                if (elapsedTime.TotalSeconds >= 10 && _gameDataService.GetEnemies().Count <= 10)
                {
                    _gameDataService.AddEnemies();
                    //Console.WriteLine($"Spawning enemy {startTime} - {DateTime.Now}. {_gameDataService.GetEnemies().Count}");
                    _gameDataService.SetGameTime(DateTime.Now);
                }
            }
            List<string> enemies = _gameDataService.GetEnemies();
            if (enemies.Count > 0)
            {
                //Console.WriteLine($"Sending Enemies {Context.ConnectionId}");
                await _gameDataService.GetObservers()[Context.ConnectionId].HandleEnemies(enemies);
            }
        }
        public async Task UpdateDeadPlayer(int id)
        {
            _gameDataService.UpdateDeadPlayer(id - 1);
            Dictionary<string, Observer> observers = _gameDataService.GetObservers();
            foreach (var observerEntry in observers)
            {
                var connectionId = observerEntry.Key;
                var observer = observerEntry.Value;

                if (connectionId != Context.ConnectionId)
                {
                    await observer.HandleMoveUpdate(_gameDataService.GetPlayerData(id - 1));
                }
            }
        }
        public async Task SendMove(int id, double x, double y)
        {
            _gameDataService.EditPlayerPosition(id - 1, x, y);
            Dictionary<string, Observer> observers = _gameDataService.GetObservers();
            foreach (var observerEntry in observers)
            {
                var connectionId = observerEntry.Key;
                var observer = observerEntry.Value;

                if (connectionId != Context.ConnectionId)
                {
                    await observer.HandleMoveUpdate(_gameDataService.GetPlayerData(id - 1));
                }
            }
        }

        public async Task SendEnemyUpdate(string enemy)
        {
            string[] partsenemy = enemy.Split(':');
            if (partsenemy.Length == 6)
            {
                int id = int.Parse(partsenemy[0]);
                string color = partsenemy[1];
                int health = int.Parse(partsenemy[4]);
                _gameDataService.UpdateEnemy(id, health);
                Dictionary<string, Observer> observers = _gameDataService.GetObservers();
                foreach (var observerEntry in observers)
                {
                    var connectionId = observerEntry.Key;
                    var observer = observerEntry.Value;
                    Console.WriteLine($"{connectionId} vs {Context.ConnectionId}");
                    if (connectionId != Context.ConnectionId)
                    {
                        await observer.HandleEnemyUpdate(id, color, health);
                    }
                }
            }
        }

        public async Task ShotFired(int player_id, double directionX, double directionY)
        {
            Dictionary<string, Observer> observers = _gameDataService.GetObservers();
            foreach (var observerEntry in observers)
            {
                var connectionId = observerEntry.Key;
                var observer = observerEntry.Value;
                if (connectionId != Context.ConnectionId)
                {
                    await observer.HandleShotFired(player_id, directionX, directionY);
                }
            }
        }

        public async Task SendDeadEnemy(string enemy)
        {
            string[] parts = enemy.Split(':');
            if (parts.Length == 6)
            {
                int id = int.Parse(parts[0]);
                string color = parts[1];
                _gameDataService.RemoveEnemy(id);
                Dictionary<string, Observer> observers = _gameDataService.GetObservers();
                foreach (var observerEntry in observers)
                {
                    var connectionId = observerEntry.Key;
                    var observer = observerEntry.Value;
                    if (connectionId != Context.ConnectionId)
                    {
                        await observer.HandleDeadEnemy(id, color);
                    }
                }
            }
        }
    }
}
