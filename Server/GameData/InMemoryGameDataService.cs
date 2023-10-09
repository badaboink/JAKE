using JAKE.classlibrary;

namespace Server.GameData
{
    public class InMemoryGameDataService : IGameDataService
    {
        private List<Player> players = new List<Player>();
        private List<Obstacle> obstacles = new List<Obstacle>();
        private List<Enemy> enemies = new List<Enemy>();
        private DateTime gametime = DateTime.Now;

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
    }
}
