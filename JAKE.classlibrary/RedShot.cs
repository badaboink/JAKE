using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JAKE.classlibrary.Patterns;

namespace JAKE.classlibrary
{
    public class RedShot : ShotDecorator
    {
        public RedShot(Shot shot) : base(shot)
        {
            ShootRed("red", 3, 18, 10);
        }
        public void ShootRed(string color, double speed, double size, double points)
        {
            Shoot(color, speed, size, points);
        }

        public override string getColor()
        {
            return wrapee.getColor();
        }
        public override double getSpeed()
        {
            return wrapee.getSpeed();
        }
        public override double getSize()
        {
            return wrapee.getSize();
        }
        public override double getPoints()
        {
            return wrapee.getPoints();
        }

    }
}
