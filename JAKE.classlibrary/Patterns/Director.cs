using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary.Patterns
{
    public class Director
    {
        private IBuilder<Player> playerBuilder;
        private IBuilder<Enemy> enemyBuilder;

        //public Director(IBuilder<Player> playerBuilder, IBuilder<Enemy> enemyBuilder)
        //{
        //    this.playerBuilder = playerBuilder;
        //    this.enemyBuilder = enemyBuilder;
        //}
        public Director(IBuilder<Player> playerBuilder)
        {
            this.playerBuilder = playerBuilder;
        }

        public Player ConstructPlayer(int id, string color)
        {
            return playerBuilder
                .New()
                .SetId(id)
                .SetColor(color)
                .Build();
        }

        public Enemy ConstructEnemy(int id, string color, string name, double x, double y)
        {
            return enemyBuilder
                .SetId(id)
                .SetColor(color)
                .SetCurrentPosition(x, y)
                .Build();
        }
    }
}
