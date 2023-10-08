using JAKE.classlibrary;
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
        [Fact]
        public void Test_DistanceFromObstacle_Returns_CorrectDistance_FromAbove()
        {
            double expectedDistance = 0;
            
            double distance = obstacle1.DistanceFromObstacle(0, -1, 0, 10, 5, 5);

            Assert.Equal(expectedDistance, distance);
        }
        // player is on the right side of the obstacle
        // moving towards it
        [Fact]
        public void Test_DistanceFromObstacle_Returns_CorrectDistance_FromLeft()
        {
            double expectedDistance = 10;

            double distance = obstacle1.DistanceFromObstacle(-1, 0, 0, 20, 5, 5);

            Assert.Equal(expectedDistance, distance);
        }
    }
}
