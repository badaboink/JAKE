using JAKE.classlibrary.Patterns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JAKE.classlibrary.Patterns.Strategies;

namespace JAKE.classlibrary.Enemies
{
    public class MiniZombie : Zombie
    {
        private IMoveStrategy movementStrategy;
        public MiniZombie(string name, int health, double x, double y, int size) : base(name, health, x, y, size)
        {
            Name = name;
            Health = health;
            X = x;
            Y = y;
            Size = size;
        }

        //public override Zombie Clone()
        //{
        //    return new MiniZombie(Name, Health, X, Y, Size);
        //}

        public override Zombie Clone()
        {
            return MemberwiseClone() as MiniZombie;
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

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Health, X, Y, Size);
        }
    }
}
