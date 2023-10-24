using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary
{
    abstract class EnemyFactory
    {
        public abstract Minion CreateMinion(int id, string color);
        public abstract Boss CreateBoss(int id, string color);
    }

    abstract class Boss : Enemy
    {
        protected Boss(int id, string color, double speed = 2, int health = 20, int size = 20) : base(id, color, speed, health, size)
        {
        }

        //big boy
        public abstract void SpawnMinion();//spawn small boyos
    }

    abstract class Minion : Enemy
    {
        //small boyes
        protected Minion(int id, string color, double speed = 2, int health = 20, int size = 20) : base(id, color, speed, health, size)
        {
        }
    }

    class ZombieFactory : EnemyFactory
    {
        public override Minion CreateMinion(int id, string color)
        {
            return new ZombieMinion(id, color);    
        }
        public override Boss CreateBoss(int id, string color)
        {
            return new ZombieBoss(id, color);
        }
    }

    class ZombieMinion : Minion
    {
        public ZombieMinion(int id, string color, double speed = 2, int health = 20, int size = 20) : base(id, color, speed, health, size)
        {
        }
    }

    class ZombieBoss : Boss
    {
        public ZombieBoss(int id, string color, double speed = 2, int health = 20, int size = 20) : base(id, color, speed, health, size)
        {
        }

        public override void SpawnMinion()
        {
            throw new NotImplementedException();//spawn small zombinos
        }
    }

    class SkeletonFactory : EnemyFactory
    {
        public override Minion CreateMinion(int id, string color)
        {
            return new SkeletonMinion(id, color);
        }
        public override Boss CreateBoss(int id, string color)
        {
            return new SkeletonBoss(id, color);
        }
    }
    class SkeletonMinion : Minion
    {
        public SkeletonMinion(int id, string color, double speed = 2, int health = 20, int size = 20) : base(id, color, speed, health, size)
        {
        }
    }
    class SkeletonBoss : Boss
    {
        public SkeletonBoss(int id, string color, double speed = 2, int health = 20, int size = 20) : base(id, color, speed, health, size)
        {
        }

        public override void SpawnMinion()
        {
            throw new NotImplementedException();//spawn small skellys
        }
    }
}
