using JAKE.classlibrary;
using JAKE.classlibrary.Enemies;

namespace Server.GameData
{
    public static class GameFunctions
    {
     
        public static List<Obstacle> GenerateObstacles()
        {
            List<Obstacle> obstacles = new();
            Random random = new();
            for (int i = 0; i < random.Next(8, 10); i++)
            {
                double widthTemp = random.Next(50, 300);
                double heightTemp = widthTemp < 150 ? random.Next(200, 300) : random.Next(50, 100);


                double xtemp = random.Next(60, (int)(1920 - widthTemp));
                double ytemp = random.Next(60, (int)(1080 - heightTemp));

                Obstacle obstacle = new(widthTemp, heightTemp, xtemp, ytemp);
                obstacles.Add(obstacle);
            }
            return obstacles;
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
