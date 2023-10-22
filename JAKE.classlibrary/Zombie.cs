using JAKE.classlibrary.Patterns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary
{
    public class Zombie
    {
        private IMoveStrategy movementStrategy;

        public string Name { get; set; }
        public int Health { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public int Size { get; set; }

        public Zombie(string name, int health, double x, double y, int size )
        {
            Name = name;
            Health = health;
            X = x;
            Y = y;
            Size = size;
        }

        public virtual Zombie Clone()
        {
            return new Zombie(Name, Health, X, Y, Size);
        }

        public virtual void TakeDamage(int damage)
        {
            Health -= damage;
            if (Health <= 0)
            {
                Console.WriteLine($"{Name} has been defeated!");
            }
        }

        public void SetCurrentPosition(double x, double y)
        {
            this.X = x;
            this.Y = y;
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
