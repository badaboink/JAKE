using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary.Patterns
{
    public class AliveState : IState
    {
        private Player player;

        public AliveState(Player player)
        {
            this.player = player;
        }
        public void setCurrentLook()
        {
            player.SetColor(player.GetPrimaryColor());
            player.SetName(player.GetPrimaryName());
        }
    }
}
