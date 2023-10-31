using JAKE.classlibrary;
using JAKE.classlibrary.Patterns;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Class_library_tests
{
    public class ObstacleTests
    {
        public static IEnumerable<object[]> TestData()
        {
            yield return new object[] { 0, -1, 0, 10, 5, 5, 0 };
            yield return new object[] { 0, -1, 0, 0, 7, 5, 10 };
            yield return new object[] { -1, -1, 5, 5, 5, 5, 5 };
        }

        private readonly Obstacle obstacle1;
        public ObstacleTests()
        {
            obstacle1 = new Obstacle(10, 10, 0, 0);
        }
        [Fact]
        public void Test_WouldOverlap_Returns_True_When_OverlapExists()
        {
            bool result = obstacle1.WouldOverlap(5, 5, 5, 5);

            Assert.True(result);
        }
        [Fact]
        public void Test_WouldOverlap_Returns_False_When_NoOverlapExists()
        {
            bool result = obstacle1.WouldOverlap(20, 20, 5, 5);

            Assert.False(result);
        }
        // obstacle is at 0;0 and 10x10
        // player is 5x5
        // player is at the bottom of the obstacle right by the edge of it
        [Theory]
        [MemberData(nameof(TestData))]
        public void Test_DistanceFromObstacle_Returns_CorrectDistance_FromAbove(int a, int b, double c, double d, double e, double f, double g)
        {
            double expectedDistance = g;
            
            double distance = obstacle1.DistanceFromObstacle(a, b, c, d, e, f);

            Assert.Equal(expectedDistance, distance);
        }
        // player is on the right side of the obstacle
        // moving towards it
        [Theory]
        [MemberData(nameof(TestData))]
        public void Test_DistanceFromObstacle_Returns_CorrectDistance_FromLeft(int a, int b, double c, double d, double e, double f, double g)
        {
            double expectedDistance = g;

            double distance = obstacle1.DistanceFromObstacle(a, b, c, d, e, f);

            Assert.Equal(expectedDistance, distance);
        }
        [Fact]
        public void WouldOverlap_ShouldReturnFalse_WhenNoOverlapOccurs()
        {
            // Arrange
            var obstacle = new Obstacle(5, 5, 10, 10);

            // Act
            bool result = obstacle.WouldOverlap(20, 20); // Coordinates that do not overlap with the obstacle

            // Assert
            Assert.False(result);
        }
        [Fact]
        public void DistanceFromObstacleX_ShouldReturnPositiveDistance_WhenPlayerMovingRight()
        {
            // Arrange
            var obstacle = new Obstacle(5, 5, 10, 10);
            double playerSpeed = 1;
            double playerDirectionX = 1;
            double playerCurrentX = 5;
            double playerWidth = 2;

            // Act
            double result = obstacle.DistanceFromObstacleX(playerSpeed, playerDirectionX, playerCurrentX, playerWidth);

            // Assert
            Assert.True(result > 0);
        }
        [Fact]
        public void DistanceFromObstacleX_ShouldReturnNegativeDistance_WhenPlayerMovingLeft()
        {
            // Arrange
            var obstacle = new Obstacle(5, 5, 10, 10);
            double playerSpeed = 1;
            double playerDirectionX = -1;
            double playerCurrentX = 5;
            double playerWidth = 2;

            // Act
            double result = obstacle.DistanceFromObstacleX(playerSpeed, playerDirectionX, playerCurrentX, playerWidth);

            // Assert
            Assert.True(result < 0);
        }

        [Fact]
        public void DistanceFromObstacleX_ShouldReturnZero_WhenPlayerNotMovingHorizontally()
        {
            // Arrange
            var obstacle = new Obstacle(5, 5, 10, 10);
            double playerSpeed = 1;
            double playerDirectionX = 0;
            double playerCurrentX = 5;
            double playerWidth = 2;

            // Act
            double result = obstacle.DistanceFromObstacleX(playerSpeed, playerDirectionX, playerCurrentX, playerWidth);

            // Assert
            Assert.Equal(0, result);
        }
    }
}
