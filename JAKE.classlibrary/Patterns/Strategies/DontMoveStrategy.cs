using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JAKE.classlibrary.Enemies;


namespace JAKE.classlibrary.Patterns.Strategies
{
    // nothing to test
    [ExcludeFromCodeCoverage]
    public class DontMoveStrategy : IMoveStrategy
    {
        public DontMoveStrategy() { }
        public void Move(Enemy enemy, List<Player> players)
        {

        }
     
    }
}
