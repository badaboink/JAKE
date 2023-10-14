using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JAKE.classlibrary.Patterns;

namespace JAKE.classlibrary
{
    public class GreenShot : ShotDecorator
    {
        public GreenShot(Shot shot) : base(shot)
        {
            ShootGreen("green", 8, 7, 4);
        }
        public void ShootGreen(string color, double speed, double size, double points)
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
