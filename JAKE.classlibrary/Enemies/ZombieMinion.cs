using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JAKE.classlibrary.Patterns.AbstractFactory;
using JAKE.classlibrary.Patterns.Strategies;


namespace JAKE.classlibrary.Enemies
{

    public class ZombieMinion : Minion
    {
        private ZombieBoss? parent = null;
        
        public ZombieMinion(int id, string color, double speed = 2, int health = 20, int size = 20) : base(id, color, speed, health, size)
        {
        }

        public override Minion? Clone()
        {
            return MemberwiseClone() as ZombieMinion;
        }

        public void SetBoss(ZombieBoss boss)
        {
            parent = boss;
            SetMovementStrategy(new CircleStrategy(3, 0));
        }
    }
}
