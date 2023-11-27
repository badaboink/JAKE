using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary
{
    [ExcludeFromCodeCoverage]
    public class Coordinates
    {
#pragma warning disable S1104 // Fields should not have public accessibility
        public double x;
#pragma warning restore S1104 // Fields should not have public accessibility
#pragma warning disable S1104 // Fields should not have public accessibility
        public double y;
#pragma warning restore S1104 // Fields should not have public accessibility

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
#pragma warning disable S1104 // Fields should not have public accessibility
        public bool trigger;
#pragma warning restore S1104 // Fields should not have public accessibility
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
