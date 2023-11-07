using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary.Collectibles
{
    public class ShieldItemDecorator : Decorator
    {
        public ShieldItemDecorator(Player player) : base(player)
        {
            player.SetLastObjectPicked("Shield Activated!");
        }

        public override (string text, float health, bool shieldOn) Display(float health, bool shield)
        {
            return base.Display(health, shield);
        }
    }
}
