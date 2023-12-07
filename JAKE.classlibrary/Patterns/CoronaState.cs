using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary.Patterns
{
    public class CoronaState : IState
    {
        private Player player;

        public CoronaState(Player player)
        {
            this.player = player;
        }
        public void setCurrentLook()
        {
            player.SetColor("olive");
            player.SetName("Kovidas!");
        }
    }
}
