using System;
using System.Collections.Generic;
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
            setColor("green");
            setSpeed(8);
            setSize(7);
            setPoints(4);
            setPosition(shot.getX(), shot.getY());
        }
    }
}
