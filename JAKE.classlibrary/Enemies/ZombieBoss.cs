using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JAKE.classlibrary.Patterns.AbstractFactory;

namespace JAKE.classlibrary.Enemies
{
    public class ZombieBoss : Boss
    {
        public ZombieBoss(int id, string color, double speed = 2, int health = 20, int size = 20) : base(id, color, speed, health, size)
        {
        }

        public override void SpawnMinion()
        {
            throw new NotImplementedException();
        }
    }
}
