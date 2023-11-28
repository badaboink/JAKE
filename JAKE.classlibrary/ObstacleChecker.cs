using JAKE.classlibrary.Patterns;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary
{
    public class ObstacleChecker
    {
        readonly List<Obstacle> obstacles;

        public ObstacleChecker(List<Obstacle> obstacles)
        {
            this.obstacles = obstacles;
        }

        [ExcludeFromCodeCoverage]
        public List<Obstacle> GetObstacles
        {
            get
            {
                return obstacles;
            }
        }
        public bool IsPointOverlapping(double x, double y)
        {
            var item = obstacles.Select(g => g).FirstOrDefault(g => g.WouldOverlap(x, y));
            return item != null;
        }

        public bool IsRectangleOverlapping(double xStart, double yStart, double xWidth, double yWidth)
        {
            var item = obstacles.Select(g => g).FirstOrDefault(g => g.WouldOverlap(xStart, yStart, xWidth, yWidth));
            return item != null;
        }

        public bool PositionNextToObstacle(Coordinates current, Coordinates direction, ref Coordinates next)
        {
            bool overlap = false;
            GameStats gameStat = GameStats.Instance;
            for (int i = 0; i < obstacles.Count; i++)
            {
                Obstacle obstacle = obstacles[i];
                if (obstacle.WouldOverlap(next.x, next.y, 50, 50))
                {
                    overlap = true;
                    next = obstacle.MoveToClosestEdge(next, 50, 50);
                }
            }
            double minX = 0; // Minimum X-coordinate
            double minY = 0; // Minimum Y-coordinate
            double maxX = gameStat.WindowWidth - 60; // Maximum X-coordinate
            double maxY = gameStat.WindowHeight - 80; // Maximum Y-coordinate

            // Ensure player stays within the boundaries
            next.x = Math.Max(minX, Math.Min(next.x, maxX));
            next.y = Math.Max(minY, Math.Min(next.y, maxY));
            return overlap;
        }
    }
}
