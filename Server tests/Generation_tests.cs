using JAKE.classlibrary;
using JAKE.classlibrary.Enemies;
using Microsoft.AspNetCore.Routing;
using Server.GameData;
using System.Collections.Generic;

namespace Server_tests
{
    public class Generation_tests
    {
        [Fact]
        public void TestGenerateObstaclesExpectedRange()
        {
            int expectedMinObstacles = 8;
            int expectedMaxObstacles = 10;

            List<Obstacle> obstacles = GameFunctions.GenerateObstacles();

            Assert.True(obstacles.Count >= expectedMinObstacles);
            Assert.True(obstacles.Count <= expectedMaxObstacles);

        }

        [Fact]
        public void TestGeneratedObstaclesMeetSizeCriteria()
        {
            List<Obstacle> obstacles = GameFunctions.GenerateObstacles();

            foreach (var obstacle in obstacles)
            {
                Assert.True(obstacle.Width >= 50 && obstacle.Width <= 300);
                Assert.True(obstacle.Height >= 50 && obstacle.Height <= 300);
                Assert.True(obstacle.PositionX >= 60 && obstacle.PositionX <= 1920 - obstacle.Width);
                Assert.True(obstacle.PositionY >= 60 && obstacle.PositionY <= 1080 - obstacle.Height);
            }
        }

        [Fact]
        public void IsPositionClearPositionIsClearReturnsTrue()
        {
            double x = 100;
            double y = 100;
            int enemySize = 20;
            List<Obstacle> obstacles = new List<Obstacle>
        {
            new Obstacle(100, 100, 500, 500),
            new Obstacle(150, 150, 800, 800)

        };

            bool isClear = GameFunctions.IsPositionClear(x, y, obstacles, enemySize);

            Assert.True(isClear, "Position should be clear, but it's not.");
        }
    }
}