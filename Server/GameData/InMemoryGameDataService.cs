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
    }
}
