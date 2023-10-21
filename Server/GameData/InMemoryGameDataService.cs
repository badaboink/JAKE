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
            return newPlayer;

        }
        public Player RemovePlayer(string connectionID)
        {
            Player playerToRemove = players.FirstOrDefault(player => player.GetConnectionId() == connectionID);
            players.Remove(playerToRemove);
            return playerToRemove;
        }
        //IMapObject healthBoost = objectFactory.CreateMapObject("healthboost");
        //IMapObject coin = objectFactory.CreateMapObject("coin");
        //IMapObject weapon = objectFactory.CreateMapObject("weapon");
        //IMapObject shield = objectFactory.CreateMapObject("shield");
        //IMapObject speedBoost = objectFactory.CreateMapObject("speedboost");

        //Player player = new Player();

        //healthBoost.Interact(player);
        //coin.Interact(player);
        //weapon.Interact(player);
        //shield.Interact(player);
        //speedBoost.Interact(player);

        //KAIP DARYT>?? 1) nurodyt points string ir is anksto tik galimos reiksmes 2)priskirt points ne per konstruktoriu o tiesiai
        //jei skirtingi laukai nei interfac'e, tai kaip juos priskirt?
        //ar lists turi but interface tipo ar klases?
        //image priskirt jau mainwindow? 
        //kurioj vietoj laikom points ir health? prie player? 
        //gamedata kuo skiriasi kiekvienam faile metodai? kur jie naudojasi?
        //gamehub sedcoins?
        

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
