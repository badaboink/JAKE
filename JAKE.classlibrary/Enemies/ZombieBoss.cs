using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JAKE.classlibrary.Patterns.AbstractFactory;
using JAKE.classlibrary.Patterns.Strategies;

namespace JAKE.classlibrary.Enemies
{
    public class ZombieBoss : Boss
    {
        private readonly List<ZombieMinion> minions = new();
        private readonly int maxMinions;
        public ZombieBoss(int id, string color, double speed = 2, int health = 20, int size = 20) : base(id, color, speed, health, size)
        {
            maxMinions = 8;
        }

        public override void SpawnMinion(int id, List<Obstacle> obstacles)
        {
            if(minions.Count >= maxMinions)
            {
                return;
            }
            double radius = 40;
            Random rand = new();
            double angle = rand.NextDouble() * Math.PI * 2;
            double xx = (GetCurrentX() + radius * Math.Cos(angle));
            double yy = (GetCurrentY() + radius * Math.Sin(angle));
            if (minions.Count < 1)
            {
              
                ZombieMinion zombieMinion = new(id, "green", 10);
                zombieMinion.SetCurrentPosition(xx, yy);
                zombieMinion.SetMovementStrategy(new CircleStrategy(radius, angle, this, obstacles));
                minions.Add(zombieMinion);
            }
            else
            {
                ZombieMinion copyZombieMinion = minions[0].DeepClone() as ZombieMinion;
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                copyZombieMinion.SetId(id);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                copyZombieMinion.SetCurrentPosition(xx, yy);
                copyZombieMinion.SetMovementStrategy(new CircleStrategy(radius, angle, this, obstacles));
                minions.Add(copyZombieMinion);
            }
        }

        public override void Hit()
        {
            base.Hit();
            foreach(var minion in minions)
            {
                minion.Hit();
            }
        }

        public List<ZombieMinion> GetMinions()
        {
            return minions;
        }

    }
}
