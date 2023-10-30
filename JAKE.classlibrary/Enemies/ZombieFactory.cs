using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JAKE.classlibrary.Patterns.AbstractFactory;


namespace JAKE.classlibrary.Enemies
{
    public class ZombieFactory : EnemyFactory
    {
        public override Boss CreateBoss(int id, string color)
        {
            return new ZombieBoss(id, color);
        }

        public override Minion CreateMinion(int id, string color)
        {
            return new ZombieMinion(id, color);
        }
    }
}
