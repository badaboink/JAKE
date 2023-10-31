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
        List<Obstacle> obstacles;

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

        public bool PositionNextToObstacle(Coordinates current, Coordinates direction, ref Coordinates next)
        {
            bool overlap = false;
            GameStats gameStat = GameStats.Instance;
            double minDistance = double.MaxValue;
            foreach (Obstacle obstacle in obstacles)
            {
                if (obstacle.WouldOverlap(next.x, next.y, 50, 50))
                {
                    overlap = true;
                    double distanceX = obstacle.DistanceFromObstacleX(gameStat, direction.x, current.x, 50);
                    double distanceY = obstacle.DistanceFromObstacleY(gameStat, direction.y, current.y, 50);
                    double distance = distanceX * distanceX + distanceY * distanceY;
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                    }
                    else
                    {
                        continue;
                    }
                    // Move player to the closest edge of the obstacle
                    next.x = current.x + distanceX;
                    next.y = current.y + distanceY;

                    // Adjust player position so that its side touches the obstacle's side
                    if (direction.x < 0)
                    {
                        // Player is moving left, adjust X position
                        next.x = current.x - distanceX;
                    }

                    if (direction.y < 0)
                    {
                        // Player is moving up, adjust Y position
                        next.y = current.y - distanceY;
                    }
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
