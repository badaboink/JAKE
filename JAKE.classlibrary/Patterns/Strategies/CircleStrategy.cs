using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JAKE.classlibrary.Enemies;

namespace JAKE.classlibrary.Patterns.Strategies
{
    // not implemented yet
    [ExcludeFromCodeCoverage]
    public class CircleStrategy : IMoveStrategy
    {
        private readonly double radius;
        private double angle;
        private readonly Enemy center;
        readonly List<Obstacle> obstacles;

        public CircleStrategy(double radius, double angle, Enemy center, List<Obstacle> obstacles)
        {
            this.radius = radius;
            this.angle = angle;
            this.center = center;
            this.obstacles = obstacles;
        }

        public void Move(Enemy enemy, List<Player> players)
        {
            if (!enemy.Trigerred)
            {
                double x = center.GetCurrentX();
                double y = center.GetCurrentY();
                double newX = x + radius * Math.Cos(angle);
                double newY = y + radius * Math.Sin(angle);
                angle += (Math.PI / 8) * (enemy.GetSpeed() / 7);
                angle = angle % (Math.PI * 2);
                enemy.SetCurrentPosition(newX, newY);
                return;
            }

            Player? closestPlayer = enemy.FindClosestPlayer(players);

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

                double newX = enemy.GetCurrentX() + directionX * enemySpeed * 2;
                double newY = enemy.GetCurrentY() + directionY * enemySpeed * 2;

                Coordinates next = new Coordinates(newX, newY);
                foreach (Obstacle obstacle in obstacles)
                {
                    if (obstacle.WouldOverlap(newX, newY, enemy.GetSize(), enemy.GetSize()))
                    {
                        next = obstacle.MoveToClosestEdge(next, enemy.GetSize(), enemy.GetSize());
                    }
                }
                enemy.SetCurrentPosition(next.x, next.y);
                
            }
        }

    }
}
