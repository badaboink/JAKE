using System;
using System.Collections.Generic;
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
            setColor("red");
            setSpeed(3);
            setSize(18);
            setPoints(10);
            setPosition(shot.getX(), shot.getY());
        }
    }
}
