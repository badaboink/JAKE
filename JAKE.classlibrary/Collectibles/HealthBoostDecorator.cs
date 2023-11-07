using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary.Collectibles
{
    public class HealthBoostDecorator : Decorator
    {
        public HealthBoostDecorator(Player player) : base(player)
        {
            player.SetLastObjectPicked("Health!");
        }
        public override (string text, float health, bool shieldOn) Display(float health, bool shield)
        {
            return base.Display(health, shield);
        }
    }
}
