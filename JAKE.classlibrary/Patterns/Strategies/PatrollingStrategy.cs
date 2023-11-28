using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JAKE.classlibrary.Enemies;


namespace JAKE.classlibrary.Patterns.Strategies
{
    public class PatrollingStrategy : IMoveStrategy
    {
        private readonly List<Obstacle> obstacles;
        private readonly Random random;
        private int directionX;
        private int directionY;
        private readonly double maxX;
        private readonly double maxY;
        private readonly double enemySpeed;
        public PatrollingStrategy(double maxX, double maxY, double enemySpeed, List<Obstacle> obstacles)
        {
            random = new Random();
            this.maxX = maxX;
            this.maxY = maxY;
            this.enemySpeed = enemySpeed;
            this.obstacles = obstacles;
            directionX = 2;
            directionY = 2;
            GenerateRandomDirection();
        }
        // is tested when testing move function
        [ExcludeFromCodeCoverage]
        private void GenerateRandomDirection()
        {
            if (directionX == 2 && directionY == 2)
            {
                switch (random.Next(1, 3))
                {
                    case 1:
                        directionX = random.Next(0, 2) == 0 ? -1 : 1;
                        directionY = 0;
                        break;
                    default:
                        directionY = random.Next(0, 2) == 0 ? -1 : 1;
                        directionX = 0;
                        break;
                }
            }
            else
            {
                HandleOtherDirection();
            }

        }

        private void HandleOtherDirection()
        {
            directionX = random.Next(0, 2) switch
            {
                0 => directionX == 1 || directionX == -1 ? 0 : -1,
                _ => directionX == 1 || directionX == -1 ? 0 : 1,
            };
            directionY = random.Next(0, 2) switch
            {
                0 => directionY == 1 || directionY == -1 ? 0 : -1,
                _ => directionY == 1 || directionY == -1 ? 0 : 1,
            };
        }

        public void Move(Enemy enemy, List<Player> players)
        {
            double newX = enemy.GetCurrentX() + directionX * enemySpeed;
            double newY = enemy.GetCurrentY() + directionY * enemySpeed;

            // Check if the new position is out of bounds
            if (newX < 0 || newX > maxX || newY < 0 || newY > maxY)
            {
                // Change direction and continue patrolling
                GenerateRandomDirection();
            }
            else
            {
                var obstacle = obstacles.Select(ob => ob).FirstOrDefault(ob => ob.WouldOverlap(newX, newY, enemy.GetSize(), enemy.GetSize()));
                if (obstacle != null)
                {
                    HandleObstacle(enemy, ref newX, ref newY, obstacle);
                }
                enemy.SetCurrentPosition(newX, newY);
            }

        }

        private void HandleObstacle(Enemy enemy, ref double newX, ref double newY, Obstacle obstacle)
        {
            double distance = obstacle.DistanceFromObstacle(directionX, directionY, enemy.GetCurrentX(), enemy.GetCurrentY(), enemy.GetSize(), enemy.GetSize());
            if (distance != 0)
            {
                newX = directionX == 0 ? enemy.GetCurrentX() : enemy.GetCurrentX() + distance;
                newY = directionY == 0 ? enemy.GetCurrentY() : enemy.GetCurrentY() + distance;
            }
            GenerateRandomDirection();
        }

        public int GetCurrentX()
        {
            return directionX;
        }
        public int GetCurrentY()
        {
            return directionY;
        }

    }
}
