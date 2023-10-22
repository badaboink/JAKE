using JAKE.classlibrary;
using JAKE.classlibrary.Patterns;
using Microsoft.AspNetCore.SignalR;
using Server.Hubs;
using System.Reflection.Emit;

namespace Server.GameData
{
    public class InMemoryGameDataService : IGameDataService
    {

        private Dictionary<string, Observer> observers = new Dictionary<string, Observer>();
        private List<Player> players = new List<Player>();
        private List<Obstacle> obstacles = new List<Obstacle>();
        private List<Enemy> enemies = new List<Enemy>();
        private DateTime gametime = DateTime.Now;

        private List<Coin> coins = new List<Coin>();
        private List<HealthBoost> healthBoosts = new List<HealthBoost>();
        private List<Shield> shields = new List<Shield>();
        private List<SpeedBoost> speedBoosts = new List<SpeedBoost>();
        private List<Weapon> weapons = new List<Weapon>();
        public MapObjectFactory objectFactory = new MapObjectFactory();

        public Director director;

        private static List<Zombie> minions = new List<Zombie>();
        private BossZombie boss = new BossZombie("", 0, 0, 0,0, minions);
        public InMemoryGameDataService()
        {
            // Generate obstacles when the service is created
            obstacles = GameFunctions.GenerateObstacles();
            var playerBuilder = new PlayerBuilder();
            director = new Director(playerBuilder);
        }
        public Player AddPlayer(string playerName, string playerColor, string connectionID)
        {
            Console.WriteLine("addplayer inmemory");
            int playerId = players.Count + 1;
            Player newPlayer = director.ConstructPlayer(playerId, playerColor);
            newPlayer.SetName(playerName);
            newPlayer.SetConnectionId(connectionID);
            players.Add(newPlayer);
            Console.WriteLine("playerscount inmemorydataservce: " + players.Count);
            return newPlayer;

        }
        public Player RemovePlayer(string connectionID)
        {
            Player playerToRemove = players.FirstOrDefault(player => player.GetConnectionId() == connectionID);
            players.Remove(playerToRemove);
            return playerToRemove;
        }
        

        public void EditPlayerPosition(int id, double x, double y)
        {
            players[id].SetCurrentPosition(x, y);
        }

        public string GetPlayerData(int id)
        {
            return players[id].ToString();
        }

        public List<string> GetPlayerList()
        {

            return players.Select(player => player.ToString()).ToList();
        }

        public string GetObstacleData()
        {
            return string.Join(",", obstacles.Select(obstacle => obstacle.ToString()));
        }
        private readonly object enemyListLock = new object();
        public Enemy AddEnemies()
        {
            lock (enemyListLock)
            {
                Enemy newEnemy = GameFunctions.GenerateEnemy(enemies.Count + 1, obstacles);
                // TODO - nzn kokios maxX ir maxY reiksmes
                newEnemy.SetMovementStrategy(new PatrollingStrategy(1920-60-newEnemy.GetSize(), 1080-80-newEnemy.GetSize(), newEnemy.GetSpeed(), obstacles));
                enemies.Add(newEnemy);
                return newEnemy;
            }
        }
        public void UpdateEnemy(int id, int health)
        {
            lock (enemyListLock)
            {
                Enemy enemyToUpdate = enemies.Find(enemy => enemy.MatchesId(id));
                if (enemyToUpdate != null)
                {
                    enemyToUpdate.SetHealth(health);
                    if (enemyToUpdate.GetCurrentMovementStrategy() is PatrollingStrategy)
                    {
                        enemyToUpdate.SetMovementStrategy(new ChasePlayerStrategy(obstacles));
                    }
                }
            }
        }
        public void RemoveEnemy(int id)
        {
            lock (enemyListLock)
            {
                Enemy enemyToRemove = enemies.FirstOrDefault(enemy => enemy.MatchesId(id));
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
                }
                return enemies.Select(enemy => enemy.ToString()).ToList();
            }
        }

        public BossZombie AddBossZombie(string name, int health)
        {

            Random random = new Random();
            BossZombie bossZombie = new BossZombie(name, health, 0,0, 70, new List<Zombie>());
            int maxAttempts = 100;
            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                double spawnX = random.Next(bossZombie.Size, 1920 - 60 - bossZombie.Size);
                double spawnY = random.Next(bossZombie.Size, 1080 - 80 - bossZombie.Size);

                bool positionClear = GameFunctions.IsPositionClear(spawnX, spawnY, obstacles, bossZombie.Size);

                if (positionClear)
                {
                    bossZombie.SetCurrentPosition(spawnX, spawnY);
                    break;
                }
            }
            // Add 8 mini zombies around the boss zombie in a circular pattern
            for (int i = 0; i < 8; i++)
            {
                double angle = 2 * Math.PI * i / 8; // Divide the circle into 8 equal parts
                bossZombie.AddMinion(new MiniZombie($"Minion {i + 1}", 50, 0, 0,10), angle, 3); // Radius set to 3 for example
            }

            var zombies = new List<Zombie>
            {
                bossZombie
            };

            bossZombie.SetMovementStrategy(new PatrollingStrategy(1920 - 60 - bossZombie.Size, 1080 - 80 - bossZombie.Size, 3, obstacles));

            boss = bossZombie;
            minions = zombies;
            return bossZombie;

        }

        public void UpdateBossZombie(int health, bool mini)
        { 
            boss.Health = health;
           // if (!mini) minions.Clear();
        }

        public void RemoveBossZombie()
        {
            boss = null;
        }
        public void RemoveMiniZombie()
        {
            minions.Clear();
        }
        public List<string> GetBossZombie()
        {
            List<string> zombies = new List<string>();
            zombies.Add(boss.ToString());
            zombies.AddRange(minions.Select(minion => minion.ToString())); // Add minions to the list

            return zombies;
        }
        public bool GetBossNull()
        {
            return boss == null;
        }
        // strategy pattern
        // strategy for movement changes after xxx amount of time has passed
        public List<string> UpdateBossZombiePosition() //List<string>
        {
            boss.Move(players);
                foreach (var minion in minions)
                {
                    minion.Move(players);
                }

            List<string> zombies = new List<string>();
            zombies.Add(boss.ToString()); 
            zombies.AddRange(minions.Select(minion => minion.ToString())); // Add minions to the list

            return zombies;

            //return boss.ToString();
        }
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
        public void RemoveObserver(string connectionId)
        {
            if (observers.ContainsKey(connectionId))
            {
                observers.Remove(connectionId);
            }
        }
        public Dictionary<string, Observer> GetObservers()
        {
            return observers;
        }
        public void UpdateDeadPlayer(int id)
        {
            players[id].SetName("DEAD");
            players[id].SetColor("Black");
        }
        private readonly object coinsListLock = new object();
        public Coin AddCoin(int points)
        {
            //Console.WriteLine("ADDCOIN inemmory");
            lock (coinsListLock)
            {
                Random random = new Random();
                double x = 0, y = 0;
                bool overlap = true;
                Coin coin = (Coin)objectFactory.CreateMapObject("coin", points);

                while (overlap)
                {
                    x = random.Next(0, 1536);
                    y = random.Next(0, 800);

                    // Check for overlap with obstacles
                    overlap = obstacles.Any(obstacle => obstacle.WouldOverlap(x, y, coin.Width, coin.Height));
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
                Coin coinToRemove = coins.FirstOrDefault(coin => coin.MatchesId(id));
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

        private readonly object healthListLock = new object();
        public HealthBoost AddHealthBoost(int health)
        {
            //Console.WriteLine("ADDCOIN inemmory");
            lock (healthListLock)
            {
                Random random = new Random();
                double x = 0, y = 0;
                bool overlap = true;
                HealthBoost healthBoost = (HealthBoost)objectFactory.CreateMapObject("healthboost", health);

                while (overlap)
                {
                    x = random.Next(0, 1536);
                    y = random.Next(0, 800);

                    // Check for overlap with obstacles
                    overlap = obstacles.Any(obstacle => obstacle.WouldOverlap(x, y, healthBoost.Width, healthBoost.Height));
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
                HealthBoost healthToRemove = healthBoosts.FirstOrDefault(health => health.MatchesId(id));
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

        private readonly object speedListLock = new object();
        public SpeedBoost AddSpeedBoost(int speed)
        {
            //Console.WriteLine("ADDCOIN inemmory");
            lock (speedListLock)
            {
                Random random = new Random();
                double x = 0, y = 0;
                bool overlap = true;
                SpeedBoost speedBoost = (SpeedBoost)objectFactory.CreateMapObject("speedboost", speed);

                while (overlap)
                {
                    x = random.Next(0, 1536);
                    y = random.Next(0, 800);

                    // Check for overlap with obstacles
                    overlap = obstacles.Any(obstacle => obstacle.WouldOverlap(x, y, speedBoost.Width, speedBoost.Height));
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
                SpeedBoost speedToRemove = speedBoosts.FirstOrDefault(speed => speed.MatchesId(id));
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

        private readonly object shieldListLock = new object();
        public Shield AddShield(int time)
        {
            //Console.WriteLine("ADDCOIN inemmory");
            lock (shieldListLock)
            {
                Random random = new Random();
                double x = 0, y = 0;
                bool overlap = true;
                Shield shield = (Shield)objectFactory.CreateMapObject("shield", time);

                while (overlap)
                {
                    x = random.Next(0, 1536);
                    y = random.Next(0, 800);

                    // Check for overlap with obstacles
                    overlap = obstacles.Any(obstacle => obstacle.WouldOverlap(x, y, shield.Width, shield.Height));
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
                Shield shieldToRemove = shields.FirstOrDefault(shield => shield.MatchesId(id));
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
