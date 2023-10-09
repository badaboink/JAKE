using JAKE.classlibrary;

namespace Server.GameData
{
    public class GameFunctions
    {
        public static List<Obstacle> GenerateObstacles()
        {
            List<Obstacle> obstacles = new List<Obstacle>();
            Random random = new Random();
            for (int i = 0; i < random.Next(8, 10); i++)
            {
                //nzn kuris geriau atrodo
                //int temp = random.Next(0, 2);
                //int widthTemp = temp == 0 ? random.Next(400, 500) : 50;
                //int heightTemp = temp == 1 ? random.Next(400, 500) : 50;

                double widthTemp = random.Next(50, 300);
                double heightTemp = widthTemp < 150 ? random.Next(200, 300) : random.Next(50, 100);

                // TO DO: atsisakau daryti packing algoritma

                double xtemp = random.Next(60, (int)(1920 - widthTemp));
                double ytemp = random.Next(60, (int)(1080 - heightTemp));

                Obstacle obstacle = new Obstacle(widthTemp, heightTemp, xtemp, ytemp);
                obstacles.Add(obstacle);
            }
            return obstacles;
        }
        public static Enemy GenerateEnemy(int id, List<Obstacle> obstacles)
        {
            Random random = new Random();
            Enemy enemy = new Enemy(id, "Blue", 10);
            int maxAttempts = 100;
            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                double spawnX = random.Next(0, 1936);
                double spawnY = random.Next(0, 1056);

                // Check if the generated position overlaps with any obstacle
                bool positionClear = IsPositionClear(spawnX, spawnY, obstacles, enemy.GetSize());

                if (positionClear)
                {
                    // Set the position and break out of the loop
                    enemy.SetCurrentPosition(spawnX, spawnY);
                    break;
                }
            }
            //double spawnX = random.Next(0, 1936);
            //double spawnY = random.Next(0, 1056);
            //enemy.SetCurrentPosition(spawnX, spawnY);
            return enemy;
        }
        public static bool IsPositionClear(double x, double y, List<Obstacle> obstacles, int enemySize)
        {
            foreach (Obstacle obstacle in obstacles)
            {
                // Check if the enemy's position overlaps with the obstacle
                if (!obstacle.WouldOverlap(x, y, enemySize, enemySize))
                {
                    // The position is clear, so continue checking other obstacles
                    continue;
                }

                // The position overlaps with an obstacle, so it's not clear
                return false;
            }

            // The position is clear of all obstacles
            return true;
        }

    }
}
