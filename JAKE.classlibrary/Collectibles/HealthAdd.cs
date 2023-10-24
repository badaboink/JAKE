using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary.Collectibles
{
    public class HealthAdd : Decorator
    {
        public HealthAdd(Player player) : base(player) { }
        public (string text, float health, bool shieldOn) DisplayHealth(float health)
        {           
            return base.Display("", health, false);          
        }
    }
}
