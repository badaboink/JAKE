using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary
{
    public abstract class Decorator : Player
    {
        protected Player wrapper;

        public Decorator(Player player)
        {
            wrapper = player;
        }

        public override (string text, float health, bool shieldOn) Display(float health, bool shield)
        {
            return wrapper.Display(health, shield);
        }
    }
}
