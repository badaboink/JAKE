using JAKE.classlibrary;

namespace Server.GameData
{
    public class InMemoryGameDataService : IGameDataService
    {

        private Dictionary<string, Observer> observers = new Dictionary<string, Observer>();
        private List<Player> players = new List<Player>();
        private List<Obstacle> obstacles = new List<Obstacle>();
        private List<Enemy> enemies = new List<Enemy>();
        private DateTime gametime = DateTime.Now;

        private List<IMapObject> coins = new List<IMapObject>();
        private List<HealthBoost> healthBoosts = new List<HealthBoost>();
        private List<Shield> shields = new List<Shield>();
        private List<SpeedBoost> speedBoosts = new List<SpeedBoost>();
        private List<Weapon> weapons = new List<Weapon>();
        public MapObjectFactory objectFactory = new MapObjectFactory();
        public InMemoryGameDataService()
        {
            // Generate obstacles when the service is created
            obstacles = GameFunctions.GenerateObstacles();
        }
        public Player AddPlayer(string playerName, string playerColor, string connectionID)
        {
            int playerId = players.Count + 1;
            Player newPlayer = new Player(playerId, playerName, playerColor);
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
        private readonly object coinsListLock = new object();
        public IMapObject AddCoin(int points)
        {
            //TODO: surast random vieta kur padet ji, kad neoverlapintu nieko kito tam taske (obstacle) visa kita gali overlapint?
            //sukurt random x ir y ir eit per obstacles sarasa ir tikrint ir tada is naujo kol neoverlapina
            lock (coinsListLock)
            {
                Random random = new Random();
                double x=0, y = 0;
                bool overlap = true;
                IMapObject coin = objectFactory.CreateMapObject(string.Format("coin{0}", points));

                while (overlap)
                {
                    x = random.Next(0, 1920);
                    y = random.Next(0, 1080); 

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
                IMapObject coinToRemove = coins.FirstOrDefault(coin => coin.MatchesId(id));
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
                return coins.Select(coin => coin.ToString()).ToList();
            }
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
                newEnemy.SetMovementStrategy(new PatrollingStrategy(1920-60-newEnemy.GetSize(), 1080-80 - newEnemy.GetSize(), newEnemy.GetSpeed(), obstacles));
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
    }
}
