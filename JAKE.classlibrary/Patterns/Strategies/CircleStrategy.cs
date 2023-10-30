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
        private double radius;
        private double angle;

        public CircleStrategy(double radius, double angle)
        {
            this.radius = radius;
            this.angle = angle;
        }

        public void Move(Enemy enemy, List<Player> players)
        {
            
        }

      
    }
}
