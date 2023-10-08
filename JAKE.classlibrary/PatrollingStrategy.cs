using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary
{
    public class PatrollingStrategy : IMoveStrategy
    {
        private List<Obstacle> obstacles;
        private Random random;
        private double directionX;
        private double directionY;
        private double maxX;
        private double maxY;
        private double enemySpeed;
        public PatrollingStrategy(double maxX, double maxY, double enemySpeed, List<Obstacle> obstacles)
        {
            this.random = new Random();
            this.maxX = maxX;
            this.maxY = maxY;
            this.enemySpeed = enemySpeed;
            this.obstacles = obstacles;
            GenerateRandomDirection();
        }
        private void GenerateRandomDirection()
        {
            // Generate random direction vector (normalized)
            directionX = random.NextDouble() * 2 - 1; // Range: [-1, 1]
            directionY = random.NextDouble() * 2 - 1; // Range: [-1, 1]

            // Normalize the direction vector
            double length = Math.Sqrt(directionX * directionX + directionY * directionY);
            if (length > 0)
            {
                directionX /= length;
                directionY /= length;
            }
        }

        public void Move(Enemy enemy, List<Player> players)
        {
            double newX = enemy.GetCurrentX() + directionX * enemySpeed;
            double newY = enemy.GetCurrentY() + directionY * enemySpeed;

            // Check if the new position is out of bounds
            if (newX < enemy.GetSize()|| newX > maxX || newY < enemy.GetSize() || newY > maxY)
            {
                // Change direction and continue patrolling
                GenerateRandomDirection();
            }
            else
            {
                foreach (Obstacle obstacle in obstacles)
                {
                    if (obstacle.WouldOverlap(newX, newY, 20, 20))
                    {
                        // Stops at the wall of the direction that it's moving towards most
                        directionX = (Math.Abs(directionX) > Math.Abs(directionY)) ? (directionX < 0 ? -1 : 1) : 0;
                        directionY = (Math.Abs(directionY) > Math.Abs(directionX)) ? (directionY < 0 ? -1 : 1) : 0;

                        double distance = obstacle.DistanceFromObstacle((int)directionX, (int)directionY, enemy.GetCurrentX(), enemy.GetCurrentY(), enemy.GetSize(), enemy.GetSize());
                        if (distance != 0)
                        {
                            newX = directionX == 0 ? enemy.GetCurrentX() : enemy.GetCurrentX() + distance;
                            newY = directionY == 0 ? enemy.GetCurrentY() : enemy.GetCurrentY() + distance;

                            enemy.SetCurrentPosition(newX, newY);
                        }
                        GenerateRandomDirection();
                        break;
                    }
                }
            }

            // Update the enemy's position
            enemy.SetCurrentPosition(newX, newY);
        }
    }
}
