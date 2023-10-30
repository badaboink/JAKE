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
        public override ZombieBoss CreateBoss(int id, string color, double speed = 2, int health = 20, int size = 20)
        {
            return new ZombieBoss(id, color, speed, health, size);
        }

        public override ZombieMinion CreateMinion(int id, string color, double speed = 2, int health = 20, int size = 20)
        {
            return new ZombieMinion(id, color, speed, health, size);
        }
    }
}
