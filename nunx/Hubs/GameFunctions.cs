using JAKE.classlibrary;

namespace Server.Hubs
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
    }
}
