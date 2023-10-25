using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JAKE.classlibrary.Enemies;
using JAKE.classlibrary.Collectibles;
using JAKE.classlibrary.Patterns;
using JAKE.classlibrary.Patterns.Strategies;

namespace JAKE.classlibrary
{
    public class Spawner
    {
        private MapObjectFactory _mapObjectFactory;
        private ZombieFactory _zombieFactory;
        private ObstacleChecker _obstacleChecker;
        int maxAttempts = 1000;
        HashSet<int> _usedIdsEnemies;
        int minId = 1;
        int maxId = Int32.MaxValue;

        public Spawner(MapObjectFactory mapObjectFactory, ZombieFactory zombieFactory, ObstacleChecker obstacleChecker)
        {
            _mapObjectFactory = mapObjectFactory;
            _zombieFactory = zombieFactory;
            _obstacleChecker = obstacleChecker;
            _usedIdsEnemies = new HashSet<int>();
        }

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
            ZombieBoss zombieBoss = _zombieFactory.CreateBoss(enemyId, "green", 10);
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
                zombieBoss.SpawnMinion(enemyId);
            }
            return zombieBoss;
        }
    }
}
