using JAKE.classlibrary;
using JAKE.classlibrary.Patterns;
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
        
        public async Task SendColor(string color, string name, string shotcolor, string shotshape)
        {
            try
            {
                Player newPlayer = _gameDataService.AddPlayer(name, color, Context.ConnectionId, shotcolor, shotshape);
                Console.WriteLine($"Player {newPlayer.ToString()}");

                Dictionary<string, Observer> observers = _gameDataService.GetObservers();

                await observers[Context.ConnectionId].GameStart(newPlayer, _gameDataService.GetObstacleData());
                List<string> playerlist = _gameDataService.GetPlayerList();
                Console.WriteLine("playerlistcount gamehub: " + playerlist.Count);
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
                if (_gameDataService.GetCoins().Count <= 10)
                {
                    _gameDataService.AddCoin(10);
                }
                if (_gameDataService.GetShields().Count <= 2)
                {
                    _gameDataService.AddShield(30);
                }
                if (_gameDataService.GetHealthBoosts().Count <= 1)
                {
                    _gameDataService.AddHealthBoost(10);
                }
                if (_gameDataService.GetSpeedBoosts().Count <= 2)
                {
                    _gameDataService.AddSpeedBoost(5);
                    Console.WriteLine("pridejo speed5");
                }
                if (_gameDataService.GetBossNull())
                {
                    _gameDataService.AddBossZombie("Sefas", 200);
                    Console.WriteLine("pridejo sefa");
                }
            }
            List<string> enemies = _gameDataService.GetEnemies();
            if (enemies.Count > 0)
            {
                //Console.WriteLine($"Sending Enemies {Context.ConnectionId}");
                await _gameDataService.GetObservers()[Context.ConnectionId].HandleEnemies(enemies);
            }
            List<string> coins = _gameDataService.GetCoins();
            if (coins.Count > 0)
            {
                await _gameDataService.GetObservers()[Context.ConnectionId].HandleCoins(coins);
            }
            List<string> shields = _gameDataService.GetShields();
            if (shields.Count > 0)
            {
                await _gameDataService.GetObservers()[Context.ConnectionId].HandleShields(shields);
            }
            List<string> healthBoosts = _gameDataService.GetHealthBoosts();
            if (healthBoosts.Count > 0)
            {
                await _gameDataService.GetObservers()[Context.ConnectionId].HandleHealthBoosts(healthBoosts);
            }
            List<string> speedBoosts = _gameDataService.GetSpeedBoosts();
            if (speedBoosts.Count > 0)
            {
                await _gameDataService.GetObservers()[Context.ConnectionId].HandleSpeedBoosts(speedBoosts);
            }
            List<string> boss = _gameDataService.GetBossZombie();
            if (boss.Count > 0)
            {
                await _gameDataService.GetObservers()[Context.ConnectionId].HandleBossZombie(boss);
            }


        }
        public async Task SendPickedCoin(string coin)
        {
            string[] parts = coin.Split(':');
            if (parts.Length == 6)
            {
                int id = int.Parse(parts[0]);
                _gameDataService.RemoveCoin(id);
                Dictionary<string, Observer> observers = _gameDataService.GetObservers();
                foreach (var observerEntry in observers)
                {
                    var observer = observerEntry.Value;     
                    await observer.HandlePickedCoin(id);
                }
            }
        }

        public async Task SendPickedShield(string shield)
        {
            string[] parts = shield.Split(':');
            if (parts.Length == 6)
            {
                int id = int.Parse(parts[0]);
                _gameDataService.RemoveShield(id);
                Dictionary<string, Observer> observers = _gameDataService.GetObservers();
                foreach (var observerEntry in observers)
                {  
                    var observer = observerEntry.Value;
                    await observer.HandlePickedShield(id);
                }
            }
        }

        public async Task SendPickedHealthBoost(string health)
        {
            string[] parts = health.Split(':');
            if (parts.Length == 6)
            {
                int id = int.Parse(parts[0]);
                _gameDataService.RemoveHealthBoost(id);
                Dictionary<string, Observer> observers = _gameDataService.GetObservers();
                foreach (var observerEntry in observers)
                {
                    var connectionId = observerEntry.Key;
                    var observer = observerEntry.Value;

                    if (connectionId != Context.ConnectionId)
                    {
                        await observer.HandlePickedHealthBoost(id);
                    }
                }
            }
        }

        public async Task SendPickedSpeedBoost(string speed)
        {
            string[] parts = speed.Split(':');
            if (parts.Length == 7)
            {
                int id = int.Parse(parts[0]);
                _gameDataService.RemoveSpeedBoost(id);
                Dictionary<string, Observer> observers = _gameDataService.GetObservers();
                foreach (var observerEntry in observers)
                {
                    var observer = observerEntry.Value;
                    await observer.HandlePickedSpeedBoost(id);  
                }
            }
        }

        public async Task UpdateDeadPlayer(int id)
        {
            _gameDataService.UpdateDeadPlayer(id);
            Dictionary<string, Observer> observers = _gameDataService.GetObservers();
            foreach (var observerEntry in observers)
            {
                var connectionId = observerEntry.Key;
                var observer = observerEntry.Value;

                if (connectionId != Context.ConnectionId)
                {
                    await observer.HandleMoveUpdate(_gameDataService.GetPlayerData(id));
                }
            }
        }
        public async Task SendMove(int id, double x, double y)
        {
            _gameDataService.EditPlayerPosition(id, x, y);
            Dictionary<string, Observer> observers = _gameDataService.GetObservers();
            foreach (var observerEntry in observers)
            {
                var connectionId = observerEntry.Key;
                var observer = observerEntry.Value;

                if (connectionId != Context.ConnectionId)
                {
                    await observer.HandleMoveUpdate(_gameDataService.GetPlayerData(id));
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

        public async Task SendDeadBossZombie(string boss)
        {
            string[] parts = boss.Split(':');
            if (parts.Length == 5)
            {
                string name = parts[0];
                _gameDataService.RemoveBossZombie();
                Dictionary<string, Observer> observers = _gameDataService.GetObservers();
                foreach (var observerEntry in observers)
                {
                    var connectionId = observerEntry.Key;
                    var observer = observerEntry.Value;
                    if (connectionId != Context.ConnectionId)
                    {
                        await observer.HandleDeadBossZombie(name);
                    }
                }
            }
        }

        public async Task SendDeadMiniZombie(string boss)
        {
            string[] parts = boss.Split(':');
            if (parts.Length == 5)
            {
                string name = parts[0];
                _gameDataService.RemoveMiniZombie();
                Dictionary<string, Observer> observers = _gameDataService.GetObservers();
                foreach (var observerEntry in observers)
                {
                    var connectionId = observerEntry.Key;
                    var observer = observerEntry.Value;
                    if (connectionId != Context.ConnectionId)
                    {
                        await observer.HandleDeadMiniZombie(name);
                    }
                }
            }
        }
    }
}
