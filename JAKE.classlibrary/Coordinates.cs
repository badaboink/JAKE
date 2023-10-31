using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary
{
    public class Coordinates
    {
        public double x;
        public double y;

        [ExcludeFromCodeCoverage]
        public Coordinates()
        {
        }

        public Coordinates(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public class Trigger
    {
        public bool trigger;
        public Trigger()
        {
            trigger = false;
        }

        public void Flip()
        {
            trigger = true;
        }
    }
}
