using JAKE.classlibrary;
using JAKE.classlibrary.Patterns;
using JAKE.client;
using JAKE.Client;
using Microsoft.AspNet.SignalR.Client.Hubs;
using Microsoft.AspNetCore.SignalR.Client;
using System.Windows.Controls;
using System.Windows.Media;

namespace Client_tests
{
    public class Player_tests
    {
        [Fact]
        public void CheckCollision_ShouldReturnTrue_ForCollidingRectangles()
        {
            // Arrange
            double x1 = 0, y1 = 0, width1 = 10, height1 = 10;
            double x2 = 5, y2 = 5, width2 = 10, height2 = 10;

            // Act
            bool result = MainWindow.CheckCollision(x1, y1, width1, height1, x2, y2, width2, height2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CheckCollision_ShouldReturnFalse_ForNonCollidingRectangles()
        {
            // Arrange
            double x1 = 0, y1 = 0, width1 = 10, height1 = 10;
            double x2 = 15, y2 = 15, width2 = 10, height2 = 10;

            // Act
            bool result = MainWindow.CheckCollision(x1, y1, width1, height1, x2, y2, width2, height2);

            // Assert
            Assert.False(result);
        }
    }
}