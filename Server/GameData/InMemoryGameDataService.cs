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
        public Player AddPlayer(string playerName, string playerColor)
        {
            int playerId = players.Count + 1;
            Player newPlayer = new Player(playerId, playerName, playerColor);
            players.Add(newPlayer);
            return newPlayer;
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
        public List<string> UpdateEnemyPositions()
        {
            lock (enemyListLock)
            {
                foreach (var enemy in enemies)
                {
                    // Find the closest player
                    Player closestPlayer = FindClosestPlayer(enemy);

                    if (closestPlayer != null)
                    {
                        // Calculate direction vector from enemy to closest player
                        double directionX = closestPlayer.GetCurrentX() - enemy.GetCurrentX();
                        double directionY = closestPlayer.GetCurrentY() - enemy.GetCurrentY();

                        // Normalize the direction vector
                        double length = Math.Sqrt(directionX * directionX + directionY * directionY);
                        if (length > 0)
                        {
                            directionX /= length;
                            directionY /= length;
                        }

                        // Define enemy movement speed
                        double enemySpeed = enemy.GetSpeed();

                        double newX = enemy.GetCurrentX() + directionX * enemySpeed;
                        double newY = enemy.GetCurrentY() + directionY * enemySpeed;

                        bool CantMove = false;
                        foreach (Obstacle obstacle in obstacles)
                        {
                            if (obstacle.WouldOverlap(newX, newY, 20, 20))
                            {
                                CantMove = true;

                                // Stops at a the wall of the direction that its moving towards most
                                directionX = (Math.Abs(directionX) > Math.Abs(directionY)) ? (directionX < 0 ? -1 : 1) : 0;
                                directionY = (Math.Abs(directionY) > Math.Abs(directionX)) ? (directionY < 0 ? -1 : 1) : 0;

                                double distance = obstacle.DistanceFromObstacle((int)directionX, (int)directionY, enemy.GetCurrentX(), enemy.GetCurrentY(), 20, 20);
                                if (distance != 0)
                                {
                                    newX = directionX == 0 ? enemy.GetCurrentX() : enemy.GetCurrentX() + distance;
                                    newY = directionY == 0 ? enemy.GetCurrentY() : enemy.GetCurrentX() + distance;

                                    enemy.SetCurrentPosition(newX, newY);
                                }
                                break;
                            }
                        }
                        // Update enemy position based on direction and speed
                        if (!CantMove)
                        {
                            enemy.SetCurrentPosition(newX, newY);
                        }
                    }
                }
                return enemies.Select(enemy => enemy.ToString()).ToList();
            }
        }
        public Player FindClosestPlayer(Enemy enemy)
        {
            Player closestPlayer = null;
            double closestDistance = double.MaxValue;

            foreach (var player in players)
            {
                // Calculate the distance between enemy and player
                double distance = Math.Sqrt(
                    Math.Pow(player.GetCurrentX() - enemy.GetCurrentX(), 2) +
                    Math.Pow(player.GetCurrentY() - enemy.GetCurrentY(), 2));

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPlayer = player;
                }
            }

            return closestPlayer;
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
