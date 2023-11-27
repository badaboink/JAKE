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

        private Dictionary<string, Observer> observers = new Dictionary<string, Observer>();
        private List<Player> players = new List<Player>();
        private List<Obstacle> obstacles = new List<Obstacle>();
        private List<Enemy> enemies = new List<Enemy>();
        private DateTime gametime = DateTime.Now;

        private List<Coin> coins = new List<Coin>();
        private List<HealthBoost> healthBoosts = new List<HealthBoost>();
        private List<Shield> shields = new List<Shield>();
        private List<SpeedBoost> speedBoosts = new List<SpeedBoost>();
        private List<Corona> coronas = new List<Corona>();
        public MapObjectFactory objectFactory = new MapObjectFactory();
        public ZombieFactory zombieFactory = new ZombieFactory();
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
        Random random = new Random();
        int minId = 1;
        int maxId = Int32.MaxValue;
        HashSet<int> usedIds = new HashSet<int>();
        HashSet<int> usedIdsEnemies = new HashSet<int>();
        public Player AddPlayer(string playerName, string playerColor, string connectionID, string shotcolor, string shotshape)
        {
            int playerId = 0;
            do
            {
                playerId = new Random().Next(minId, maxId);
            } while (usedIds.Contains(playerId));
            usedIds.Add(playerId);
            Player newPlayer = new Player(playerId, playerName, playerColor, shotcolor, shotshape);
            newPlayer.SetName(playerName);
            newPlayer.SetConnectionId(connectionID);
            players.Add(newPlayer);
            return newPlayer;

        }
        public Player RemovePlayer(string connectionID)
        {
            Player playerToRemove = players.FirstOrDefault(player => player.GetConnectionId() == connectionID);
            usedIds.Remove(playerToRemove.GetId());
            players.Remove(playerToRemove);
            return playerToRemove;
        }
        

        public void EditPlayerPosition(int id, double x, double y)
        {
            Player playerToEdit = players.FirstOrDefault(p => p.GetId() == id);
            playerToEdit.SetCurrentPosition(x, y);
        }

        public string GetPlayerData(int id)
        {
            Player player = players.FirstOrDefault(p => p.GetId() == id);

            return player.ToString();
        }

        public List<string> GetPlayerList()
        {

            return players.Select(player => player.ToString()).ToList();
        }
        private DateTime startTime = DateTime.Now;
        public async Task InfectCorona(int id, double x, double y)
        {
            PlayerCollection playerNetwork = new PlayerCollection(players);
            //startTime = DateTime.Now;
            int toInfectId = -1;

            // Assume you have a corona-infected player named 'coronaInfectedPlayer'
            Player coronaInfectedPlayer = players.FirstOrDefault(p => p.GetId() == id);
            int index = players.FindIndex(p => p.GetId() == id);

            // Check if the player is found
            if (index != -1)
            {
                // Modify the state of the found player directly in the list
                players[index].state = "corona";  // Replace "new_state" with the desired state
            }

            // Use the PlayerNetwork to create an iterator for nearby players
            //IPlayerIterator nearbyPlayersIterator = playerNetwork.CreateNearbyPlayersIterator(coronaInfectedPlayer);
            IPlayerIterator nearbyPlayersIterator = playerNetwork.CreateNearbyPlayersIterator(coronaInfectedPlayer);

            // Access the start time from the iterator
            if (nearbyPlayersIterator is NearbyPlayerIterator nearbyPlayerIteratorInstance)
            {
                startTime = nearbyPlayerIteratorInstance.getStartTime();
                Console.WriteLine("Start Time: " + startTime);
            }
            else
            {
                Console.WriteLine("Iterator does not support getting start time.");
            }

            // Iterate through nearby players using the iterator
            Player nearbyPlayer;
            //while ((DateTime.Now - startTime).TotalSeconds < 10)
            //{
            //    nearbyPlayer = nearbyPlayersIterator.GetNext();
            //    // Process or do something with the 'nearbyPlayer'
            //    Console.WriteLine("Found nearby player: " + nearbyPlayer);
            //}
            //TODO: SU LAIKU TIKRINT DAR??
            Console.WriteLine("corona id: " + coronaInfectedPlayer.GetId());
            int counter = 0;
            while(((nearbyPlayer = nearbyPlayersIterator.GetNext())!=null && (DateTime.Now - startTime).TotalSeconds < 3)) //|| counter <= 10
            {
                toInfectId = nearbyPlayer.GetId();
                Console.WriteLine("toinfect coord: " + nearbyPlayer.GetCurrentX() + "  " + nearbyPlayer.GetCurrentY());
                counter++;
                foreach (var observerEntry in observers)
                {
                    var observer = observerEntry.Value;
                    Console.WriteLine("toinfect id: " + toInfectId);
                    await observer.HandleSpreadCorona(toInfectId);
                }
                //return toInfectId;
                //ARBA: kiekvienam rastam iskviecia funkcija? kuri eina i client side ir mazina health, bet jau cia tur4t7 ir eit i ja??
            }
           
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
                Enemy newEnemy = spawner.SpawnEnemy();
                newEnemy.SetMovementStrategy(new PatrollingStrategy(1920 - 60 - newEnemy.GetSize(), 1080 - 80 - newEnemy.GetSize(), newEnemy.GetSpeed(), obstacles));
                enemies.Add(newEnemy);
                return newEnemy;
            }
        }
        List<ZombieMinion> tempminions;
        public Enemy AddZombieBoss()
        {
            lock (enemyListLock)
            {
                ZombieBoss newEnemy = spawner.SpawnZombieBoss();
                newEnemy.SetMovementStrategy(new PatrollingStrategy(1920 - 60 - newEnemy.GetSize(), 1080 - 80 - newEnemy.GetSize(), newEnemy.GetSpeed(), obstacles));
                enemies.Add(newEnemy);
                List<ZombieMinion> minions = newEnemy.GetMinions();
                tempminions = newEnemy.GetMinions();
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
                Enemy enemyToUpdate = enemies.Find(enemy => enemy.MatchesId(id));
                if (enemyToUpdate != null)
                {
                    enemyToUpdate.SetHealth(health);
                    IMoveStrategy currentStrategy = enemyToUpdate.GetCurrentMovementStrategy();
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
                    Debug.WriteLine("{0} {1}", enemy.GetCurrentMovementStrategy().GetType().ToString(), enemy.ToString());
                }
                return enemies.Select(enemy => enemy.ToString()).ToList();
            }
        }

        public Coin returnCoin(int id)
        {
            return coins.FirstOrDefault(coin => coin.id == id);
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
            Player playerToUpdate = players.FirstOrDefault(p => p.GetId() == id);

            playerToUpdate.SetName("DEAD");
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
                Console.WriteLine("coronacount: " + coronas.Count);
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
