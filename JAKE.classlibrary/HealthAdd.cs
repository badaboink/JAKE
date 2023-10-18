using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary
{
    public class HealthAdd : Decorator
    {
        public HealthAdd(Player player) : base(player) { }
        public (string text, bool healthVisibility, bool shieldOn) DisplayHealth()
        {           
            return base.Display("", true, false);          
        }

    }
}
