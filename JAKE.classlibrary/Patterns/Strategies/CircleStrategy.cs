using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JAKE.classlibrary.Enemies;

namespace JAKE.classlibrary.Patterns.Strategies
{
    public class CircleStrategy : IMoveStrategy
    {
        private double radius;
        private double angle;
        private Enemy center;

        public CircleStrategy(double radius, double angle, Enemy center)
        {
            this.radius = radius;
            this.angle = angle;
            this.center = center;
        }

        public void Move(Enemy enemy, List<Player> players)
        {
            double x = center.GetCurrentX();
            double y = center.GetCurrentY();
            double newX = x + radius * Math.Cos(angle);
            double newY = y + radius * Math.Sin(angle);
            angle += (Math.PI / 8) * (enemy.GetSpeed() / 7);
            angle = angle % (Math.PI * 2);
            enemy.SetCurrentPosition(newX, newY);
        }

    }
}
