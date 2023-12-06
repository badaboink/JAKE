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
        private readonly List<Obstacle> obstacles;
        public ChasePlayerStrategy(List<Obstacle> obstacles)
        {
            this.obstacles = obstacles;

        }
        public void Move(Enemy enemy, List<Player> players)
        {
            Player? closestPlayer = enemy.FindClosestPlayer(players);

            if (closestPlayer != null)
            {
                Coordinates enemyCurrent = new(enemy.GetCurrentX(), enemy.GetCurrentY());
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

                // Define enemy movement speed
                double enemySpeed = enemy.GetSpeed();

                double newX = enemyCurrent.x + directionX * enemySpeed;
                double newY = enemyCurrent.y + directionY * enemySpeed;
                Coordinates next = new(newX, newY);
                foreach (var obstacle in from Obstacle obstacle in obstacles
                                         where obstacle.WouldOverlap(newX, newY, enemy.GetSize(), enemy.GetSize())
                                         select obstacle)
                {
                    next = obstacle.MoveToClosestEdge(next, enemy.GetSize(), enemy.GetSize());
                }

                enemy.SetCurrentPosition(next.x, next.y);
            }
        }
    }
}
