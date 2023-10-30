using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary.Patterns.AbstractFactory
{
    public abstract class EnemyFactory
    {

        protected EnemyFactory()
        {
        }

        public abstract Minion CreateMinion(int id, string color, double speed, int health, int size);
        public abstract Boss CreateBoss(int id, string color, double speed, int health, int size);
        
    }
}
