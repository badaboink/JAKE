using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JAKE.classlibrary.Enemies;
using JAKE.classlibrary.Collectibles;
using JAKE.classlibrary.Patterns;
using JAKE.classlibrary.Patterns.Strategies;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace JAKE.classlibrary
{
    public class Spawner
    {
        private readonly MapObjectFactory _mapObjectFactory;
        private readonly ZombieFactory _zombieFactory;
        private readonly ObstacleChecker _obstacleChecker;
        readonly int maxAttempts = 1000;
        readonly HashSet<int> _usedIdsEnemies;
        readonly int minId = 1;
        readonly int maxId = Int32.MaxValue;

        public Spawner(MapObjectFactory mapObjectFactory, ZombieFactory zombieFactory, ObstacleChecker obstacleChecker)
        {
            _mapObjectFactory = mapObjectFactory;
            _zombieFactory = zombieFactory;
            _obstacleChecker = obstacleChecker;
            _usedIdsEnemies = new HashSet<int>();
        }
        [ExcludeFromCodeCoverage]
        public void RemoveId(int id)
        {
            _usedIdsEnemies.Remove(id);
        }
        public Enemy SpawnEnemy()
        {
            int enemyId = 0;
            do
            {
                enemyId = new Random().Next(minId, maxId);
            } while (_usedIdsEnemies.Contains(enemyId));
            _usedIdsEnemies.Add(enemyId);
            Random random = new Random();
            Enemy enemy = new Enemy(enemyId, "Blue", 10);
            int maxAttempts = 100;
            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                double spawnX = random.Next(enemy.GetSize(), 1920 - 60 - enemy.GetSize());
                double spawnY = random.Next(enemy.GetSize(), 1080 - 80 - enemy.GetSize());
                if (!_obstacleChecker.IsRectangleOverlapping(spawnX, spawnY, enemy.GetSize(), enemy.GetSize()))
                {
                    enemy.SetCurrentPosition(spawnX, spawnY);
                    break;
                }
            }
            return enemy;
        }

        public ZombieMinion SpawnZombieMinion()
        {
            int enemyId = 0;
            do
            {
                enemyId = new Random().Next(minId, maxId);
            } while (_usedIdsEnemies.Contains(enemyId));
            _usedIdsEnemies.Add(enemyId);
            ZombieMinion zombieMinion = _zombieFactory.CreateMinion(enemyId, "green", 10);
            Random random = new Random();
            int attempt;
            for (attempt = 0; attempt < maxAttempts; attempt++)
            {
                double spawnX = random.Next(zombieMinion.GetSize(), 1920 - 60 - zombieMinion.GetSize());
                double spawnY = random.Next(zombieMinion.GetSize(), 1080 - 80 - zombieMinion.GetSize());

                if (!_obstacleChecker.IsRectangleOverlapping(spawnX, spawnY, zombieMinion.GetSize(), zombieMinion.GetSize()))
                {
                    zombieMinion.SetCurrentPosition(spawnX, spawnY);
                    break;
                }
            }
            return zombieMinion;
        }

        public ZombieBoss SpawnZombieBoss()
        {
            int enemyId = 0;
            do
            {
                enemyId = new Random().Next(minId, maxId);
            } while (_usedIdsEnemies.Contains(enemyId));
            _usedIdsEnemies.Add(enemyId);
            ZombieBoss zombieBoss = _zombieFactory.CreateBoss(enemyId, "green", 10, 100, 40);
            Random random = new Random();
            int attempt;
            for (attempt = 0; attempt < maxAttempts; attempt++)
            {
                double spawnX = random.Next(zombieBoss.GetSize(), 1920 - 60 - zombieBoss.GetSize());
                double spawnY = random.Next(zombieBoss.GetSize(), 1080 - 80 - zombieBoss.GetSize());

                if (!_obstacleChecker.IsRectangleOverlapping(spawnX, spawnY, zombieBoss.GetSize(), zombieBoss.GetSize()))
                {
                    zombieBoss.SetCurrentPosition(spawnX, spawnY);
                    break;
                }
            }
            for(int i = 0; i < 8; i++)
            {
                do
                {
                    enemyId = new Random().Next(minId, maxId);
                } while (_usedIdsEnemies.Contains(enemyId));
                _usedIdsEnemies.Add(enemyId);
                zombieBoss.SpawnMinion(enemyId, _obstacleChecker.GetObstacles);
            }
            return zombieBoss;
        }
    }
}
