using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary.Patterns.AbstractFactory
{
    public abstract class EnemyFactory
    {
        public abstract Minion CreateMinion(int id, string color);
        public abstract Boss CreateBoss(int id, string color);
    }
}
