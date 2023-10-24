using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary.Patterns
{
    public class DontMoveStrategy : IMoveStrategy
    {
        public DontMoveStrategy() { }
        public void Move(Enemy enemy, List<Player> players)
        {

        }
        public void MoveZombie(Zombie zombie, List<Player> players)
        {

        }
    }
}
