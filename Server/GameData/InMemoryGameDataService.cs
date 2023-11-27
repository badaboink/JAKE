using JAKE.classlibrary;
using JAKE.classlibrary.Patterns;
using JAKE.classlibrary.Patterns.Strategies;
using JAKE.classlibrary.Enemies;
using JAKE.classlibrary.Collectibles;
using Microsoft.AspNetCore.SignalR;
using Server.Hubs;
using System.Reflection.Emit;
using System.Diagnostics;

namespace Server.GameData
{
    public class InMemoryGameDataService : IGameDataService
    {

        private readonly Dictionary<string, Observer> observers = new();
        private readonly List<Player> players = new();
        private readonly List<Obstacle> obstacles;
        private readonly List<Enemy> enemies = new();
        private DateTime gametime = DateTime.Now;

        private readonly List<Coin> coins = new();
        private readonly List<HealthBoost> healthBoosts = new();
        private readonly List<Shield> shields = new();
        private readonly List<SpeedBoost> speedBoosts = new();
        public MapObjectFactory objectFactory = new();
        public ZombieFactory zombieFactory = new();
        public ObstacleChecker obstacleChecker;
        public Spawner spawner;
        private int bossId = -1;
        public InMemoryGameDataService()
        {
            // Generate obstacles when the service is created
            obstacles = GameFunctions.GenerateObstacles();
            obstacleChecker = new ObstacleChecker(obstacles);
            spawner = new Spawner(objectFactory, zombieFactory, obstacleChecker);
        }
        readonly Random random = new();
        readonly int minId = 1;
        readonly int maxId = Int32.MaxValue;
        readonly HashSet<int> usedIds = new();
        public Player AddPlayer(string playerName, string playerColor, string connectionID, string shotcolor, string shotshape)
        {
            int playerId;
            do
            {
                playerId = new Random().Next(minId, maxId);
            } while (usedIds.Contains(playerId));
            usedIds.Add(playerId);
            Player newPlayer = new(playerId, playerName, playerColor, shotcolor, shotshape);
            newPlayer.SetName(playerName);
            newPlayer.SetConnectionId(connectionID);
            players.Add(newPlayer);
            return newPlayer;

        }
        public Player RemovePlayer(string connectionID)
        {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            Player playerToRemove = players.Find(player => player.GetConnectionId() == connectionID);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            _ = usedIds.Remove(item: playerToRemove.GetId());
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            players.Remove(playerToRemove);
            return playerToRemove;
        }
        

        public void EditPlayerPosition(int id, double x, double y)
        {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            Player playerToEdit = players.Find(p => p.GetId() == id);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
            playerToEdit.SetCurrentPosition(x, y);
        }

        public string GetPlayerData(int id)
        {
            Player player = players.Find(p => p.GetId() == id);

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            return player.ToString();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }

        public List<string> GetPlayerList()
        {

            return players.Select(player => player.ToString()).ToList();
        }

        public string GetObstacleData()
        {
            return string.Join(",", obstacles.Select(obstacle => obstacle.ToString()));
        }
        private readonly object enemyListLock = new();
        public Enemy AddEnemies()
        {
            lock (enemyListLock)
            {
                Enemy newEnemy = spawner.SpawnEnemy();
                newEnemy.SetMovementStrategy(new PatrollingStrategy(1920 - 60 - newEnemy.GetSize(), 1080 - 80 - newEnemy.GetSize(), newEnemy.GetSpeed(), obstacles));
                enemies.Add(newEnemy);
                return newEnemy;
            }
        }

        public Enemy AddZombieBoss()
        {
            lock (enemyListLock)
            {
                ZombieBoss newEnemy = spawner.SpawnZombieBoss();
                newEnemy.SetMovementStrategy(new PatrollingStrategy(1920 - 60 - newEnemy.GetSize(), 1080 - 80 - newEnemy.GetSize(), newEnemy.GetSpeed(), obstacles));
                enemies.Add(newEnemy);
                List<ZombieMinion> minions = newEnemy.GetMinions();
                foreach (ZombieMinion minion in minions)
                {
                    Console.WriteLine("original minion in zombieBoss list hash: " + minion.GetHashCode());
                    Enemy copy = minion.ShallowClone();
                    Console.WriteLine("shallow copy minion in enemies list hash: " + copy.GetHashCode());
                    enemies.Add(copy); //minion.ShallowClone()

                    minion.SetMovementStrategy(new ChasePlayerStrategy(obstacles));
                }
                bossId = newEnemy.GetId();
                return newEnemy;
            }
        }
        public bool IsBossAlive()
        {
            return bossId >= 0;
        }

        public void UpdateEnemy(int id, int health)
        {
            lock (enemyListLock)
            {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                Enemy enemyToUpdate = enemies.Find(enemy => enemy.MatchesId(id));
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                if (enemyToUpdate != null)
                {
                    enemyToUpdate.SetHealth(health);
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                    IMoveStrategy currentStrategy = enemyToUpdate.GetCurrentMovementStrategy();
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                    if (currentStrategy is PatrollingStrategy || currentStrategy is CircleStrategy)
                    {
                        enemyToUpdate.SetMovementStrategy(new ChasePlayerStrategy(obstacles));
                    }
                    enemyToUpdate.Hit();
                }
            }
        }
        public void RemoveEnemy(int id)
        {
            lock (enemyListLock)
            {
                spawner.RemoveId(id);
                if(id == bossId)
                {
                    bossId = -1;
                }
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                Enemy enemyToRemove = enemies.Find(enemy => enemy.MatchesId(id));
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                if (enemyToRemove != null)
                {
                    enemies.Remove(enemyToRemove);
                }
            }
        }
        public List<string> GetEnemies()
        {
            lock (enemyListLock)
            {
                return enemies.Select(enemy => enemy.ToString()).ToList();
            }
        }
        // strategy pattern
        // strategy for movement changes after xxx amount of time has passed
        public List<string> UpdateEnemyPositions()
        {
            lock (enemyListLock)
            {
                foreach (var enemy in enemies)
                { 
                    enemy.Move(players);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    Debug.WriteLine("{0} {1}", enemy.GetCurrentMovementStrategy().GetType().ToString(), enemy.ToString());
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                }
                return enemies.Select(enemy => enemy.ToString()).ToList();
            }
        }

#pragma warning disable CS8603 // Possible null reference return.
        public Coin ReturnCoin(int id) => coins.Find(coin => coin.id == id);
#pragma warning restore CS8603 // Possible null reference return.
        public DateTime GetCurrentGameTime()
        {
            return gametime;
        }
        public void SetGameTime(DateTime gametime)
        {
            this.gametime = gametime;
        }
        public void AddObserver(string connectionID, Observer observer)
        {
            observers[connectionID] = observer;
        }
        public void RemoveObserver(string connectionID)
        {
            if (observers.ContainsKey(connectionID))
            {
                observers.Remove(connectionID);
            }
        }
        public Dictionary<string, Observer> GetObservers()
        {
            return observers;
        }
        public void UpdateDeadPlayer(int id)
        {
            Player playerToUpdate = players.Find(p => p.GetId() == id);

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            playerToUpdate.SetName("DEAD");
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            playerToUpdate.SetColor("Black");
        }
        private readonly object coinsListLock = new();
        public Coin AddCoin(int points)
        {
            lock (coinsListLock)
            {
                double x = 0, y = 0;
                bool overlap = true;
                Coin coin = (Coin)objectFactory.CreateMapObject("coin", points);

                while (overlap)
                {
                    x = random.Next(0, 1536);
                    y = random.Next(0, 800);

                    // Check for overlap with obstacles
                    overlap = obstacles.Exists(obstacle => obstacle.WouldOverlap(x, y, coin.Width, coin.Height));
                }

                coin.SetPosition(x, y);
                coin.id = coins.Count + 1;
                coins.Add(coin);
                return coin;
            }
        }

        public void RemoveCoin(int id)
        {
            lock (coinsListLock)
            {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                Coin coinToRemove = coins.Find(coin => coin.MatchesId(id));
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                if (coinToRemove != null)
                {
                    coins.Remove(coinToRemove);
                }
            }
        }
        public List<string> GetCoins()
        {
            lock (coinsListLock)
            {
                Console.WriteLine("getcoins() inmemory");
                return coins.Select(coin => coin.ToString()).ToList();
            }
        }

        private readonly object healthListLock = new();
        public HealthBoost AddHealthBoost(int health)
        {
            lock (healthListLock)
            {
                double x = 0, y = 0;
                bool overlap = true;
                HealthBoost healthBoost = (HealthBoost)objectFactory.CreateMapObject("healthboost", health);

                while (overlap)
                {
                    x = random.Next(0, 1536);
                    y = random.Next(0, 800);

                    // Check for overlap with obstacles
                    overlap = obstacles.Exists(obstacle => obstacle.WouldOverlap(x, y, healthBoost.Width, healthBoost.Height));
                }

                healthBoost.SetPosition(x, y);
                healthBoost.id = healthBoosts.Count + 1;
                healthBoosts.Add(healthBoost);
                return healthBoost;
            }
        }

        public void RemoveHealthBoost(int id)
        {
            lock (healthListLock)
            {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                HealthBoost healthToRemove = healthBoosts.Find(health => health.MatchesId(id));
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                if (healthToRemove != null)
                {
                    healthBoosts.Remove(healthToRemove);
                }
            }
        }
        public List<string> GetHealthBoosts()
        {
            lock (healthListLock)
            {
                Console.WriteLine("HealthBoostget() inmemory");
                return healthBoosts.Select(healthBoost => healthBoost.ToString()).ToList();
            }
        }

        private readonly object speedListLock = new();
        public SpeedBoost AddSpeedBoost(int speed)
        {
            lock (speedListLock)
            {
                double x = 0, y = 0;
                bool overlap = true;
                SpeedBoost speedBoost = (SpeedBoost)objectFactory.CreateMapObject("speedboost", speed);

                while (overlap)
                {
                    x = random.Next(0, 1536);
                    y = random.Next(0, 800);

                    // Check for overlap with obstacles
                    overlap = obstacles.Exists(obstacle => obstacle.WouldOverlap(x, y, speedBoost.Width, speedBoost.Height));
                }

                speedBoost.SetPosition(x, y);
                speedBoost.id = speedBoosts.Count + 1;
                speedBoosts.Add(speedBoost);
                return speedBoost;
            }
        }

        public void RemoveSpeedBoost(int id)
        {
            lock (speedListLock)
            {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                SpeedBoost speedToRemove = speedBoosts.Find(speed => speed.MatchesId(id));
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                if (speedToRemove != null)
                {
                    speedBoosts.Remove(speedToRemove);
                }
            }
        }
        public List<string> GetSpeedBoosts()
        {
            lock (speedListLock)
            {
                Console.WriteLine("speedBoostget() inmemory");
                return speedBoosts.Select(speedBoost => speedBoost.ToString()).ToList();
            }
        }

        private readonly object shieldListLock = new();
        public Shield AddShield(int time)
        {
            lock (shieldListLock)
            {
                double x = 0, y = 0;
                bool overlap = true;
                Shield shield = (Shield)objectFactory.CreateMapObject("shield", time);

                while (overlap)
                {
                    x = random.Next(0, 1536);
                    y = random.Next(0, 800);

                    // Check for overlap with obstacles
                    overlap = obstacles.Exists(obstacle => obstacle.WouldOverlap(x, y, shield.Width, shield.Height));
                }

                shield.SetPosition(x, y);
                shield.id = shields.Count + 1;
                shields.Add(shield);
                return shield;
            }
        }

        public void RemoveShield(int id)
        {
            lock (shieldListLock)
            {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                Shield shieldToRemove = shields.Find(shield => shield.MatchesId(id));
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                if (shieldToRemove != null)
                {
                    shields.Remove(shieldToRemove);
                }
            }
        }
        public List<string> GetShields()
        {
            lock (shieldListLock)
            {
                Console.WriteLine("getshields() inmemory");
                return shields.Select(shield => shield.ToString()).ToList();
            }
        }

    }
}
