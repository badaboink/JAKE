using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JAKE.classlibrary.Enemies;


namespace JAKE.classlibrary.Patterns.Strategies
{
    public class ChasePlayerStrategy : IMoveStrategy
    {
        private List<Obstacle> obstacles;
        private ObstacleChecker checker;
        public ChasePlayerStrategy(List<Obstacle> obstacles)
        {
            this.obstacles = obstacles;
            this.checker = new ObstacleChecker(obstacles);

        }
        public void Move(Enemy enemy, List<Player> players)
        {
            Player? closestPlayer = enemy.FindClosestPlayer(players);

            if (closestPlayer != null)
            {
                Coordinates enemyCurrent = new Coordinates(enemy.GetCurrentX(), enemy.GetCurrentY());
                // Calculate direction vector from enemy to closest player
                double directionX = closestPlayer.GetCurrentX() - enemyCurrent.x;
                double directionY = closestPlayer.GetCurrentY() - enemyCurrent.y;
                // Normalize the direction vector
                double length = Math.Sqrt(directionX * directionX + directionY * directionY);
                if (length > 0)
                {
                    directionX /= length;
                    directionY /= length;
                }
                Coordinates direction = new Coordinates(directionX, directionY);

                // Define enemy movement speed
                double enemySpeed = enemy.GetSpeed();

                double newX = enemyCurrent.x + directionX * enemySpeed;
                double newY = enemyCurrent.y + directionY * enemySpeed;
                Coordinates nextCoords = new Coordinates(newX, newY);
                //checker.PositionNextToObstacle(enemyCurrent, direction, ref nextCoords);
                //enemy.SetCurrentPosition(nextCoords.x, nextCoords.y);
                bool CantMove = false;
                foreach (Obstacle obstacle in obstacles)
                {
                    if (obstacle.WouldOverlap(newX, newY, enemy.GetSize(), enemy.GetSize()))
                    {
                        CantMove = true;

                        // Stops at the wall of the direction that it's moving towards most
                        directionX = Math.Abs(directionX) > Math.Abs(directionY) ? directionX < 0 ? -1 : 1 : 0;
                        directionY = Math.Abs(directionY) > Math.Abs(directionX) ? directionY < 0 ? -1 : 1 : 0;

                        double distance = obstacle.DistanceFromObstacle((int)directionX, (int)directionY, enemy.GetCurrentX(), enemy.GetCurrentY(), enemy.GetSize(), enemy.GetSize());
                        if (distance != 0)
                        {
                            newX = directionX == 0 ? enemy.GetCurrentX() : enemy.GetCurrentX() + distance;
                            newY = directionY == 0 ? enemy.GetCurrentY() : enemy.GetCurrentY() + distance;

                            enemy.SetCurrentPosition(newX, newY);
                        }
                        break;
                    }
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
