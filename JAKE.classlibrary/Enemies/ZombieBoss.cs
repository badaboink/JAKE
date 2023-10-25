using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JAKE.classlibrary.Patterns.AbstractFactory;
using JAKE.classlibrary.Patterns.Strategies;

namespace JAKE.classlibrary.Enemies
{
    public class ZombieBoss : Boss
    {
        private List<ZombieMinion> minions = new List<ZombieMinion>();
        private int maxMinions;
        public ZombieBoss(int id, string color, double speed = 2, int health = 20, int size = 20) : base(id, color, speed, health, size)
        {
            maxMinions = 8;
        }

        public override void SpawnMinion(int id)
        {
            if(minions.Count >= maxMinions)
            {
                return;
            }
            double radius = 40;
            Random rand = new Random();
            double angle = rand.NextDouble() * Math.PI * 2;
            double xx = (GetCurrentX() + radius * Math.Cos(angle));
            double yy = (GetCurrentY() + radius * Math.Sin(angle));
            if (minions.Count < 1)
            {
                ZombieMinion zombieMinion = new ZombieMinion(id, "green", 10);
                zombieMinion.SetCurrentPosition(xx, yy);
                zombieMinion.SetMovementStrategy(new CircleStrategy(radius, angle, this));
                minions.Add(zombieMinion);
            }
            else
            {
                ZombieMinion copyZombieMinion = minions[0].Clone() as ZombieMinion;
                copyZombieMinion.SetId(id);
                copyZombieMinion.SetCurrentPosition(xx, yy);
                copyZombieMinion.SetMovementStrategy(new CircleStrategy(radius, angle, this));
                minions.Add(copyZombieMinion);
            }
        }

        public List<ZombieMinion> GetMinions()
        {
            return minions;
        }

    }
}
