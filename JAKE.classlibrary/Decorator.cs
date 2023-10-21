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
        public virtual (string text, float health, bool shieldOn) Display(string text, float health, bool shieldOn)
        {
            return wrapper.Display(text, health, shieldOn);
        }
    }
}
