using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary
{
    public class ShieldOn : Decorator
    {
        public ShieldOn(Player player) : base(player) { }
        public (string text, float health, bool shieldOn) DisplayShield()
        {          
            return base.Display("", 0, true);        
        }
        public (string text, float health, bool shieldOn) HideShield()
        {
            return base.Display("", 0, false);
        }
    }
}
