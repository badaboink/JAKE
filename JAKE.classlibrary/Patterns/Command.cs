using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace JAKE.classlibrary.Patterns
{
    public abstract class Command
    {
        protected Player player;
        protected Command(Player player)
        {
            this.player = player;
        }
        public abstract bool Execute();
        public abstract void Undo();
        public bool Check(List<Obstacle> obstacles, ref double newX, ref double newY, double windowWidth, double windowHeight)
        {
            bool overlap = false;
            GameStats gameStat = GameStats.Instance;
            double minDistance = double.MaxValue;
            foreach (Obstacle obstacle in obstacles)
            {
                if (obstacle.WouldOverlap(newX, newY, 50, 50))
                {
                    overlap = true;
                    double distanceX = obstacle.DistanceFromObstacleX(gameStat.PlayerSpeed, player.GetDirectionX(), player.GetCurrentX(), 50);
                    double distanceY = obstacle.DistanceFromObstacleY(gameStat.PlayerSpeed, player.GetDirectionY(), player.GetCurrentY(), 50);
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
                    newX = player.GetCurrentX() + distanceX;
                    newY = player.GetCurrentY() + distanceY;

                    // Adjust player position so that its side touches the obstacle's side
                    if (player.GetDirectionX() < 0)
                    {
                        // Player is moving left, adjust X position
                        newX = player.GetCurrentX() - distanceX;
                    }

                    if (player.GetDirectionY() < 0)
                    {
                        // Player is moving up, adjust Y position
                        newY = player.GetCurrentY() - distanceY;
                    }
                }
            }
            double minX = 0; // Minimum X-coordinate
            double minY = 0; // Minimum Y-coordinate
            double maxX = windowWidth - 60; // Maximum X-coordinate
            double maxY = windowHeight - 80; // Maximum Y-coordinate

            // Ensure player stays within the boundaries
            newX = Math.Max(minX, Math.Min(newX, maxX));
            newY = Math.Max(minY, Math.Min(newY, maxY));
            return overlap;
        }
    }
}
