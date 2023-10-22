using JAKE.classlibrary.Patterns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary
{
    public class MiniZombie : Zombie
    {
        private IMoveStrategy movementStrategy;
        public MiniZombie(string name, int health, double x, double y, int size) : base(name, health, x, y, size)
        {
        }

        public override Zombie Clone()
        {
            return new MiniZombie(Name, Health, X, Y, Size);
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
