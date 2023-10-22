using JAKE.classlibrary.Patterns;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace JAKE.classlibrary
{
    public class Base : Decorator
    {
        public Base(Player player): base(player) { }

        public (string text, float health, bool shieldOn) DisplayObject(string mapObj)
        {
            if (mapObj == "coin")
            {
                return base.Display("+10 POINTS!", 0, false);
            }
            else if (mapObj == "shield")
            {
                return base.Display("Shield Activated!", 0, false);
            }
            else if (mapObj == "speed")
            {
                return base.Display("TUTUTURU MAX VERSTAPPEN", 0, false);
            }
            else
            {
                return base.Display("Health!", 0, false);
            }
        }
    }
}
