using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JAKE.classlibrary.Patterns;

namespace JAKE.classlibrary
{
    public class BlueShot : ShotDecorator
    {
        public BlueShot(Shot shot) : base(shot)
        {
            setColor("blue");
            setSpeed(5);
            setSize(11);
            setPoints(5);
            setPosition(shot.getX(), shot.getY());
        }
    }
}
