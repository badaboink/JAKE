using JAKE.classlibrary.Enemies;
using JAKE.classlibrary.Patterns;
using JAKE.classlibrary;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.GameData;

namespace Class_library_tests
{
    public class SpawnerTests
    {
        private MapObjectFactory mapObjectFactory;
        private ZombieFactory zombieFactory;
        private ObstacleChecker obstacleChecker;
        private Spawner spawner;

        public SpawnerTests()
        {
            // Set up your factories and obstacle checker here
            mapObjectFactory = new MapObjectFactory();
            zombieFactory = new ZombieFactory();
            List<Obstacle> obstacles = GameFunctions.GenerateObstacles();
            obstacleChecker = new ObstacleChecker(obstacles);

            // Create a Spawner instance with the initialized factories and obstacle checker
            spawner = new Spawner(mapObjectFactory, zombieFactory, obstacleChecker);
        }
        [Fact]
        public void SpawnEnemy_ReturnsValidEnemy()
        {
            Enemy enemy = spawner.SpawnEnemy();

            Assert.NotNull(enemy);
            Assert.True(enemy.GetId() > 0);
            Assert.Equal("Blue", enemy.GetColor()); 
        }
        [Fact]
        public void SpawnZombieMinion_ReturnsValidZombieMinion()
        {
            ZombieMinion zombieMinion = spawner.SpawnZombieMinion();

            Assert.NotNull(zombieMinion);
            Assert.True(zombieMinion.GetId() > 0);
            Assert.Equal("green", zombieMinion.GetColor());
        }
        [Fact]
        public void SpawnZombieBoss_ReturnsValidZombieBoss()
        {
            ZombieBoss zombieBoss = spawner.SpawnZombieBoss();

            Assert.NotNull(zombieBoss);
            Assert.True(zombieBoss.GetId() > 0);
            Assert.Equal("green", zombieBoss.GetColor());
        }
    }
}
