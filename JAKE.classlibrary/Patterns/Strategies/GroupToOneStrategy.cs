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
                var obstacle = obstacles.Select(ob => ob).FirstOrDefault(ob => ob.WouldOverlap(newX, newY, enemy.GetSize(), enemy.GetSize()));
                if (obstacle != null)
                {
                    CantMove = HandleObstacle(enemy, ref directionX, ref directionY, ref newX, ref newY, obstacle);
                }
                // Update enemy position based on direction and speed
                if (!CantMove)
                {
                    enemy.SetCurrentPosition(newX, newY);
                }
            }
        }

        private static bool HandleObstacle(Enemy enemy, ref double directionX, ref double directionY, ref double newX, ref double newY, Obstacle obstacle)
        {
            bool CantMove = true;
            // Stops at the wall of the direction that it's moving towards most
            directionX = directionX switch
            {
                < 0 => Math.Abs(directionX) > Math.Abs(directionY) ? -1 : 0,// Stops at the wall of the direction that it's moving towards most
                _ => Math.Abs(directionX) > Math.Abs(directionY) ? 1 : 0,// Stops at the wall of the direction that it's moving towards most
            };
            directionY = directionY switch
            {
                < 0 => Math.Abs(directionY) > Math.Abs(directionX) ? -1 : 0,
                _ => Math.Abs(directionY) > Math.Abs(directionX) ? 1 : 0,
            };
            double distance = obstacle.DistanceFromObstacle((int)directionX, (int)directionY, enemy.GetCurrentX(), enemy.GetCurrentY(), enemy.GetSize(), enemy.GetSize());
            if (distance != 0)
            {
                newX = directionX == 0 ? enemy.GetCurrentX() : enemy.GetCurrentX() + distance;
                newY = directionY == 0 ? enemy.GetCurrentY() : enemy.GetCurrentY() + distance;
                enemy.SetCurrentPosition(newX, newY);
            }

            return CantMove;
        }
    }
}
