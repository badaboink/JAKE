using JAKE.classlibrary;
using JAKE.Client;
using JAKE.client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.Windows.Media;

namespace Class_library_tests
{
    public class ShootingTests
    {
        [Fact]

        public void SingleShot_ShouldCreateValidShot()
        {
            // Arrange
            Shot shot;

            //MainWindow a = new MainWindow();

            // Act
            MainWindow.SingleShot(10,10,10,10, out shot);

            // Assert
            Assert.NotNull(shot);
            Assert.Equal(10, shot.getSize());
            Assert.Equal("red", shot.getColor());
            Assert.Equal(10, shot.getX());
            Assert.Equal(10, shot.getY());

        }

        [Fact]
        public void RemoveShot_ShouldSetShotToNullWhenOverlapWithObstacle()
        {
            // Arrange
            double obstacleWidth = 10;
            double obstacleHeight = 10;
            double obstacleX = 5;
            double obstacleY = 5;
            double playerX = 5;
            double playerY = 5;
            double playerSize = 5;

            Obstacle obstacle = new Obstacle(obstacleWidth, obstacleHeight, obstacleX, obstacleY);
            Shot shot = new Shot(5, "red", 5, 10);

            // Act
            Shot result = MainWindow.RemoveShot(shot, playerX, playerY, obstacle, playerSize);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void RemoveShot_ShouldNotSetShotToNullWhenNoOverlapWithObstacle()
        {
            // Arrange
            double obstacleWidth = 10;
            double obstacleHeight = 10;
            double obstacleX = 20;
            double obstacleY = 20;
            double playerX = 5;
            double playerY = 5;
            double playerSize = 5;

            Obstacle obstacle = new Obstacle(obstacleWidth, obstacleHeight, obstacleX, obstacleY);
            Shot shot = new Shot(5, "red", 5, 10);

            // Act
            Shot result = MainWindow.RemoveShot(shot, playerX, playerY, obstacle, playerSize);

            // Assert
            Assert.NotNull(result);
        }
       
    }
}
