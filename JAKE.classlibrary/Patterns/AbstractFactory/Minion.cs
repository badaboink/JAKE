using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JAKE.classlibrary.Enemies;

namespace JAKE.classlibrary.Patterns.AbstractFactory
{
    public abstract class Minion : Enemy
    {

        protected Minion(int id, string color, double speed = 2, int health = 20, int size = 20, int points = 5) : base(id, color, speed, health, size, points)
        {
        }

    }
}
