using JAKE.classlibrary.Patterns;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary
{
    public class BossZombie : Zombie
    {
        public List<Zombie> minions;
        private IMoveStrategy movementStrategy;

        public BossZombie(string name, int health, double x, double y, int size, List<Zombie> minions) : base(name, health, x, y, size)
        {
            this.minions = minions;
        }

        
        public override Zombie Clone()
        {
            List<Zombie> clonedMinions = new List<Zombie>();
            foreach (var minion in minions)
            {
                clonedMinions.Add(minion.Clone());
            }

            return new BossZombie(Name, Health, X, Y, Size, clonedMinions);
        }

        public void AddMinion(MiniZombie minion, double angle, double radius)
        {
            // Calculate minion position around the boss zombie
            int x = (int)(X + radius * Math.Cos(angle));
            int y = (int)(Y + radius * Math.Sin(angle));

            minion.SetCurrentPosition(x, y);

            minions.Add(minion);
        }

        public override void TakeDamage(int damage)
        {
            // Damage the boss zombie
            base.TakeDamage(damage);

            // Remove all minions if any minion is shot
            bool minionShot = false;
            foreach (var minion in minions)
            {
                if (minion.Health <= 0)
                {
                    minionShot = true;
                    break;
                }
            }

            if (minionShot)
            {
                Debug.WriteLine("A minion has been shot! Removing all minions.");
                minions.Clear();
            }
        }


        public void SetMovementStrategy(IMoveStrategy movementStrategy)
        {
            this.movementStrategy = movementStrategy;
        }
        public void Move(List<Player> players)
        {
            if (movementStrategy != null)
            {
                movementStrategy.MoveZombie(this, players);
            }
        }

        public override string ToString()
        {
            return $"{Name}:{X}:{Y}:{Size}:{Health}";
        }
    }

}
