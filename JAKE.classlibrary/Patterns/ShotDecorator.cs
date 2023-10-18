using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary.Patterns
{
    public abstract class ShotDecorator : Shot
    {
        protected Shot wrapee;

        public ShotDecorator(Shot shot) : base()
        {
            wrapee = shot;
        }

        public void Shoot(string color, double speed, double size, double points)
        {

            wrapee.setShot(color, speed, size, points);

        }
    }
}
