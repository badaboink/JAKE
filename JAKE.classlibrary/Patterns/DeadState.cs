using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary.Patterns
{
    public class DeadState :IState
    {
        private Player player;

        public DeadState(Player player)
        {
            this.player = player;
        }
        public void setCurrentLook()
        {
            player.SetColor("Black");
            player.SetName("DEAD :(");
        }
    }
}
