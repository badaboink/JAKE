using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JAKE.classlibrary.Patterns;

namespace JAKE.classlibrary
{
    public class TriangleShot : Shot
    {
        public TriangleShot(Shot shot, IColor color) : base(shot)
        {
            base.setShape("triangle");
            base.setColor(color);
            base.setSpeed(5);
            base.setSize(10);
            base.setPoints(5);
        }
    }
}
