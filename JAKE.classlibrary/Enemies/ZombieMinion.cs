using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JAKE.classlibrary.Patterns;
using JAKE.classlibrary.Patterns.AbstractFactory;
using JAKE.classlibrary.Patterns.Strategies;


namespace JAKE.classlibrary.Enemies
{
    //IPrototype
    public class ZombieMinion : Minion  
    {
        
        public ZombieMinion(int id, string color, double speed = 2, int health = 20, int size = 20) : base(id, color, speed, health, size)
        {
        }

        public Minion? DeepClone()
        {
            ZombieMinion clone = new(this.GetId(), this.GetColor(), this.GetSpeed(), this.GetHealth(), this.GetSize());
            clone.SetCurrentPosition(this.GetCurrentX(), this.GetCurrentY());
#pragma warning disable CS8604 // Possible null reference argument.
            clone.SetMovementStrategy(this.GetCurrentMovementStrategy());
#pragma warning restore CS8604 // Possible null reference argument.
            return clone;
        }

    }
}
