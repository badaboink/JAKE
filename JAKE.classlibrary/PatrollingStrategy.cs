﻿using System;
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
        private int directionX;
        private int directionY;
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
            this.directionX = 2;
            this.directionY = 2;
            GenerateRandomDirection();
        }
        private void GenerateRandomDirection()
        {           
            if (directionX ==2 && directionY==2)
            {
                if (random.Next(1, 3) == 1)
                {
                    directionX = random.Next(0, 2) == 0 ? -1 : 1;
                    directionY = 0;
                }
                else
                {
                    directionY = random.Next(0, 2) == 0 ? -1 : 1;
                    directionX = 0;
                }
            }
            else
            {
                directionX = directionX == 1 || directionX == -1 ? 0 : random.Next(0, 2) == 0 ? -1 : 1;
                directionY = directionY == 1 || directionY == -1 ? 0 : random.Next(0, 2) == 0 ? -1 : 1;
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

                        double distance = obstacle.DistanceFromObstacle(directionX, directionY, enemy.GetCurrentX(), enemy.GetCurrentY(), enemy.GetSize(), enemy.GetSize());
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

                enemy.SetCurrentPosition(newX, newY);
            }

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
