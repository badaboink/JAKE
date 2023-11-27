using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JAKE.classlibrary.Enemies;


namespace JAKE.classlibrary.Patterns.Strategies
{
    public class GroupToOneStrategy : IMoveStrategy
    {
        private readonly List<Obstacle> obstacles;
        private readonly Enemy King;
        public GroupToOneStrategy(List<Obstacle> obstacles, Enemy king)
        {
            this.obstacles = obstacles;
            this.King = king;
        }
        public void Move(Enemy enemy, List<Player> players)
        {
            Player? closestPlayer = enemy.FindClosestPlayer(players);

            if (closestPlayer != null)
            {
                // Calculate direction vector from enemy to closest player
                double directionX = King.GetCurrentX() - enemy.GetCurrentX();
                double directionY = King.GetCurrentY() - enemy.GetCurrentY();

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
                foreach (var obstacle in from Obstacle obstacle in obstacles
                                         where obstacle.WouldOverlap(newX, newY, enemy.GetSize(), enemy.GetSize())
                                         select obstacle)
                {
                    CantMove = true;
                    // Stops at the wall of the direction that it's moving towards most
                    if (directionX < 0)
                    {

                        // Stops at the wall of the direction that it's moving towards most
                        directionX = Math.Abs(directionX) > Math.Abs(directionY) ? -1 : 0;
                    }
                    else
                    {

                        // Stops at the wall of the direction that it's moving towards most
                        directionX = Math.Abs(directionX) > Math.Abs(directionY) ? 1 : 0;
                    }

                    if (directionY < 0)
                    {
                        directionY = Math.Abs(directionY) > Math.Abs(directionX) ? -1 : 0;
                    }
                    else
                    {
                        directionY = Math.Abs(directionY) > Math.Abs(directionX) ? 1 : 0;
                    }

                    double distance = obstacle.DistanceFromObstacle((int)directionX, (int)directionY, enemy.GetCurrentX(), enemy.GetCurrentY(), enemy.GetSize(), enemy.GetSize());
                    if (distance != 0)
                    {
                        newX = directionX == 0 ? enemy.GetCurrentX() : enemy.GetCurrentX() + distance;
                        newY = directionY == 0 ? enemy.GetCurrentY() : enemy.GetCurrentY() + distance;

                        enemy.SetCurrentPosition(newX, newY);
                    }

                    break;
                }
                // Update enemy position based on direction and speed
                if (!CantMove)
                {
                    enemy.SetCurrentPosition(newX, newY);
                }
            }
        }

    }
}
