using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary.Patterns
{
    public class ChaseAndHopStrategy : IMoveStrategy
    {
        private List<Obstacle> obstacles;
        public ChaseAndHopStrategy(List<Obstacle> obstacles)
        {
            this.obstacles = obstacles;
        }
        public void Move(Enemy enemy, List<Player> players)
        {
            Player closestPlayer = enemy.FindClosestPlayer(players);

            if (closestPlayer != null)
            {
                // Calculate direction vector from enemy to closest player
                double directionX = closestPlayer.GetCurrentX() - enemy.GetCurrentX();
                double directionY = closestPlayer.GetCurrentY() - enemy.GetCurrentY();

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

                double jumpHeight = 60.0;
                double jumpDistance = 60.0;

                bool needToJump = false;
                foreach (Obstacle obstacle in obstacles)
                {
                    if (obstacle.WouldOverlap(newX, newY, enemy.GetSize(), enemy.GetSize()))
                    {
                        needToJump = true;
                        bool canJump = true;
                        double jumpX = newX + directionX * jumpDistance;
                        double jumpY = newY + directionY * jumpDistance - jumpHeight;
                        if (obstacle.WouldOverlap(jumpX, jumpY, enemy.GetSize(), enemy.GetSize()))
                        {
                            canJump = false;
                            break;
                        }
                        if (canJump)
                        {
                            enemy.SetMovementStrategy(new HoppingStrategy(directionX, directionY, obstacle, obstacles));
                        }
                        break;
                    }
                }
                if(!needToJump && enemy.GetCurrentMovementStrategy() is ChaseAndHopStrategy)
                {
                    enemy.SetCurrentPosition(newX, newY);
                }
            }
        }
    }
}
