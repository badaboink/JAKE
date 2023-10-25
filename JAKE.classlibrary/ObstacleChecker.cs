using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary
{
    public class ObstacleChecker
    {
        List<Obstacle> obstacles;

        public ObstacleChecker(List<Obstacle> obstacles)
        {
            this.obstacles = obstacles;
        }

        public bool IsPointOverlapping(double x, double y)
        {
            foreach (Obstacle obstacle in obstacles)
            {
                if (obstacle.WouldOverlap(x, y))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsRectangleOverlapping(double xStart, double yStart, double xWidth, double yWidth)
        {
            foreach (Obstacle obstacle in obstacles)
            {
                if (obstacle.WouldOverlap(xStart, yStart, xWidth, yWidth))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
