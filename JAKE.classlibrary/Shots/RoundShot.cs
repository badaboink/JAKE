using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JAKE.classlibrary.Patterns;

namespace JAKE.classlibrary
{
    public class RoundShot : Shot
    {
       public RoundShot(Shot shot, IColor color) : base(shot)
        {
            base.setShape("round");
            base.setColor(color);
            base.setSpeed(5);
            base.setSize(10);
            base.setPoints(5);
        }
    }
}
