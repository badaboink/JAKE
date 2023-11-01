using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary.Collectibles
{
    public class SpeedDecorator : Decorator
    {
        public SpeedDecorator(Player player) : base(player)
        {
            player.SetLastObjectPicked("TUTUTURU Max Verstappen!");
        }
        public override (string text, float health, bool shieldOn) Display(float health)
        {
            return base.Display(health);
        }
    }
}
