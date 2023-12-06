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
        private List<Corona> coronas = new List<Corona>();
        public ObstacleChecker obstacleChecker;
        public Spawner spawner;
        private int bossId = -1;
        private int level = 1;
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
            Player playerToRemove = players.Find(player => player.GetConnectionId() == connectionID);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            _ = usedIds.Remove(item: playerToRemove.GetId());
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            players.Remove(playerToRemove);
            return playerToRemove;
        }
        

        public void EditPlayerPosition(int id, double x, double y)
        {
            Player playerToEdit = players.FirstOrDefault(p => p.GetId() == id);
            playerToEdit.SetCurrentPosition(x, y);
        }

        public void EditPlayerState(int id, string state, string color)
        {

            Player playerToEdit = players.FirstOrDefault(p => p.GetId() == id);
            if (state == "corona")
            {
                playerToEdit.state = "alive";
                playerToEdit.SetColor(color);
            }

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
        public List<Player> InfectCorona(int id, double x, double y)
        {
            PlayerCollection playerNetwork = new PlayerCollection(players);
            List<Player> toInfectPlayers = new List<Player>();

            // Assume you have a corona-infected player named 'coronaInfectedPlayer'
            Player coronaInfectedPlayer = players.FirstOrDefault(p => p.GetId() == id);


            int index = players.FindIndex(p => p.GetId() == id);
            Console.WriteLine("index: " + index);

            // Check if the player is found
            coronaInfectedPlayer.SetColor("Lime");
            Console.WriteLine("lime plyz1: " + coronaInfectedPlayer.GetColor());
            if (index != -1)
            {
                // Modify the state of the found player directly in the list
                players[index].state = "corona";  // Replace "new_state" with the desired state
                players[index].SetColor("Lime");
                Console.WriteLine("lime plyz2: " + players[index].GetColor());
            }
            

            // Use the PlayerNetwork to create an iterator for nearby players
            IPlayerIterator nearbyPlayersIterator = playerNetwork.CreateNearbyPlayersIterator(coronaInfectedPlayer);


            Player nearbyPlayer;
            Console.WriteLine("coronaorigin id: " + coronaInfectedPlayer.GetId());
            while(((nearbyPlayer = nearbyPlayersIterator.GetNext())!=null)) 
            {
                Console.WriteLine("nearbyplayerstate: " + nearbyPlayer.state);
                if(nearbyPlayer.state != "corona")
                {
                    toInfectPlayers.Add(nearbyPlayer);
                    Console.WriteLine("toinfect color: " + nearbyPlayer.GetColor());
                }

            }

            return toInfectPlayers;
           
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
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                    Enemy copy = minion.ShallowClone();
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8604 // Possible null reference argument.
                    enemies.Add(copy); //minion.ShallowClone()
#pragma warning restore CS8604 // Possible null reference argument.

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
        private readonly object coinsListLock = new object();
        private readonly object coronalock = new object();
        public Corona AddCorona()
        {
            lock (coronalock)
            {
                Random random = new Random();
                double x = 0, y = 0;
                bool overlap = true;
                Corona corona = (Corona)objectFactory.CreateMapObject("corona", 0);

                while (overlap)
                {
                    x = random.Next(0, 1536);
                    y = random.Next(0, 800);

                    // Check for overlap with obstacles
                    overlap = obstacles.Any(obstacle => obstacle.WouldOverlap(x, y, corona.Width, corona.Height));
                }

                corona.SetPosition(x, y);
                corona.id = coronas.Count + 1;
                coronas.Add(corona);
                return corona;
            }
        }
        public void RemoveCorona(int id)
        {
            lock (coronalock)
            {
                Corona coronaToRemove = coronas.FirstOrDefault(corona => corona.MatchesId(id));
                if (coronaToRemove != null)
                {
                    coronas.Remove(coronaToRemove);
                }
            }
        }
        public List<string> GetCoronas()
        {
            lock (coronalock)
            {
                return coronas.Select(corona => corona.ToString()).ToList();
            }
        }

        public int GetLevel()
        {
            return level;
        }

        public void SetLevel(int level)
        {
            this.level = level;
        }
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

        public void UpdateCoin(int id, int points)
        {
            lock (coinsListLock)
            {
                Coin coinToUpdate = coins.Find(coin => coin.MatchesId(id));
                if (coinToUpdate != null)
                {
                    coinToUpdate.Points = points;
                }
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
                    Console.WriteLine("removed coin");
                    Console.WriteLine("coins.count: " + coins.Count);
                }
            }
        }
        public List<string> GetCoins()
        {
            lock (coinsListLock)
            {
                //Console.WriteLine("getcoins() inmemory");
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
                //Console.WriteLine("HealthBoostget() inmemory");
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
                //Console.WriteLine("speedBoostget() inmemory");
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
               // Console.WriteLine("getshields() inmemory");
                return shields.Select(shield => shield.ToString()).ToList();
            }
        }

    }
}
