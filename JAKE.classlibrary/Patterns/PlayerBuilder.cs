using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary.Patterns
{
    public class PlayerBuilder : IBuilder<Player>
    {
        private Player player = new Player();

        public IBuilder<Player> SetId(int id)
        {
            player.SetId(id);
            return this;
        }

        public IBuilder<Player> SetColor(string color)
        {
            player.SetColor(color);
            return this;
        }

        public IBuilder<Player> SetCurrentPosition(double x, double y)
        {
            player.SetCurrentPosition(x, y);
            return this;
        }

        public Player Build()
        {
            return player;
        }
    }
}
