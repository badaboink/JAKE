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
using JAKE.classlibrary.Patterns;
using JAKE.classlibrary.Shots;
using JAKE.classlibrary.Colors;

namespace Class_library_tests
{
    public class ShootingTests
    {
        [Fact]

        public void SingleShot_ShouldCreateValidShot()
        {
            // Arrange
            Shot shot;


            // Act
            MainWindow.SingleShot(10,10,10,10, "red", "triangle", out shot);  // spalvos kodas cia (melyna)

            // Assert
            Assert.NotNull(shot);
            Assert.Equal(10, shot.getSize());     // toks dydis #FF0000FF spalvos suvio
            Assert.Equal("red", shot.getColor().GetColor());
            Assert.Equal("triangle", shot.getShape());
            Assert.Equal(10, shot.getX());
            Assert.Equal(10, shot.getY());
            Assert.Equal(5, shot.getPoints());
            Assert.Equal(5, shot.getSpeed());

            // jeigu testas nepraeina vadinasi nustatyta kitoks shape ir color MainWindow
            // kol kas shape ir color keiciami paciam kode

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
            IColor color = new RedColor();
            Shot shot = new Shot(color, 5, 10, 5);
            shot = new RoundShot(shot);

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
            IColor color = new RedColor();
            Shot shot = new Shot(color, 5, 10, 5);
            shot = new TriangleShot(shot);
            // Act
            Shot result = MainWindow.RemoveShot(shot, playerX, playerY, obstacle, playerSize);

            // Assert
            Assert.NotNull(result);
        }
    }
}
