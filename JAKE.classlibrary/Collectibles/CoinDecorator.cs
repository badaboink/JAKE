using JAKE.classlibrary.Patterns;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace JAKE.classlibrary.Collectibles
{
    public class CoinDecorator : Decorator
    {
        public CoinDecorator(Player player) : base(player)
        {
            player.SetLastObjectPicked("+10 Points!");
        }

        public override (string text, float health, bool shieldOn) Display(float health, bool shield)
        {           
            return base.Display(health, shield);
        }
    }
}
