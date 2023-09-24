using JAKE.classlibrary;

namespace Server.GameData
{
    public class InMemoryGameDataService : IGameDataService
    {
        private List<Player> players = new List<Player>();
        private List<Obstacle> obstacles = new List<Obstacle>();
        private List<Enemy> enemies = new List<Enemy>();

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
        public Enemy AddEnemies()
        {
            Enemy newEnemy = GameFunctions.GenerateEnemy(enemies.Count + 1);
            enemies.Add(newEnemy);
            return newEnemy;
        }
        public List<string> GetEnemies()
        {
            return enemies.Select(enemy => enemy.ToString()).ToList();
        }
        public List<string> UpdateEnemyPositions()
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

                    // Update enemy position based on direction and speed
                    double newX = enemy.GetCurrentX() + directionX * enemySpeed;
                    double newY = enemy.GetCurrentY() + directionY * enemySpeed;

                    enemy.SetCurrentPosition(newX, newY);
                }
            }
            return enemies.Select(enemy => enemy.ToString()).ToList();
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
    }
}
