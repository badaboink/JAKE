using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary
{
    internal class HealthBoost : IMapObject
    {
        public void Interact(Player player)
        {
            // Implement health boost logic
            player.IncreaseHealth(50); //galimai is singleton pasiimt player ir pakeist health
        }
    }
}
