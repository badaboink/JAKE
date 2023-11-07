using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary.Collectibles
{
    [ExcludeFromCodeCoverage]
    public class HealthDecorator : Decorator
    {
        public HealthDecorator(Player player) : base(player) 
        { 
        }
        public override (string text, float health, bool shieldOn) Display(float health, bool shield)
        {           
            return base.Display(health, shield);          
        }
    }
}
