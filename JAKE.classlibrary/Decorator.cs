using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary
{
    public class Decorator
    {
        protected Player wrapper;
        public Decorator(Player player)
        {
            wrapper = player;
        }
        public virtual (string text, bool healthVisibility, bool shieldOn) Display(string text, bool visibleHealth, bool shieldOn)
        {
            return wrapper.Display(text, visibleHealth, shieldOn);
        }
    }
}
