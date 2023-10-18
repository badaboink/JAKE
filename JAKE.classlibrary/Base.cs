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

        public (string text, bool healthVisibility, bool shieldOn) DisplayObject(string mapObj)
        {
            if (mapObj == "coin")
            {
                return base.Display("+10 POINTS!", false, false);
            }
            else if (mapObj == "shield")
            {
                return base.Display("Shield Activated!", false, false);
            }
            else
            {
                return base.Display("TUTUTURU MAX VERSTAPPEN", false, false);
            }
        }
    }
}
