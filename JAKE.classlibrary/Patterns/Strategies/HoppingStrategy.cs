using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JAKE.classlibrary.Enemies;


namespace JAKE.classlibrary.Patterns.Strategies
{
    public class HoppingStrategy : IMoveStrategy
    {
        private double moveDirectionX;
        private double moveDirectionY;
        private Obstacle obstacle;
        private List<Obstacle> obstacles;

        public HoppingStrategy(double moveDirectionX, double moveDirectionY, Obstacle obstacle, List<Obstacle> obstacles)
        {
            this.moveDirectionX = moveDirectionX;
            this.moveDirectionY = moveDirectionY;
            this.obstacle = obstacle;
            this.obstacles = obstacles;
        }

        public void Move(Enemy enemy, List<Player> players)
        {
            double newX = enemy.GetCurrentX() + moveDirectionX * enemy.GetSpeed() * 3;
            double newY = enemy.GetCurrentY() + moveDirectionY * enemy.GetSpeed() * 3;
            if (!obstacle.WouldOverlap(newX, newY, enemy.GetSize(), enemy.GetSize()))
            {
                enemy.SetCurrentPosition(newX, newY);
                enemy.SetMovementStrategy(new ChaseAndHopStrategy(obstacles));
            }
            else
            {
                enemy.SetCurrentPosition(newX, newY);
            }
        }
       
    }
}
