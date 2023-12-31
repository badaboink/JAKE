﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JAKE.classlibrary.Enemies;


namespace JAKE.classlibrary.Patterns.Strategies
{
    public class ChaseAndHopStrategy : IMoveStrategy
    {
        private readonly List<Obstacle> obstacles;
        public ChaseAndHopStrategy(List<Obstacle> obstacles)
        {
            this.obstacles = obstacles;
        }
        public void Move(Enemy enemy, List<Player> players)
        {
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

                double newX = enemy.GetCurrentX() + directionX * enemySpeed;
                double newY = enemy.GetCurrentY() + directionY * enemySpeed;

                double jumpHeight = 60.0;
                double jumpDistance = 60.0;

                bool needToJump = false;
                double jumpX = newX + directionX * jumpDistance;
                double jumpY = newY + directionY * jumpDistance - jumpHeight;
                var obstacle = obstacles.Select(ob => ob).FirstOrDefault(ob => ob.WouldOverlap(newX, newY, enemy.GetSize(), enemy.GetSize()));
                if (obstacle != null)
                {
                    needToJump = true;
                    if (!obstacle.WouldOverlap(jumpX, jumpY, enemy.GetSize(), enemy.GetSize()))
                    {
                        enemy.SetMovementStrategy(new HoppingStrategy(directionX, directionY, obstacle, obstacles));
                    }
                }
                

                if (!needToJump && enemy.GetCurrentMovementStrategy() is ChaseAndHopStrategy)
                {
                    enemy.SetCurrentPosition(newX, newY);
                }
            }
        }
    }
}
