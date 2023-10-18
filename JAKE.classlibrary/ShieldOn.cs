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
        public (string text, bool healthVisibility, bool shieldOn) DisplayShield()
        {          
            return base.Display("", false, true);        
        }
    }
}
