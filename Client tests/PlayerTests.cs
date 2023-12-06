using JAKE.classlibrary;
using JAKE.classlibrary.Collectibles;
using JAKE.classlibrary.Patterns;
using JAKE.client;
using JAKE.Client;
using Microsoft.AspNet.SignalR.Client.Hubs;
using Microsoft.AspNetCore.SignalR.Client;
using System.Windows.Controls;
using System.Windows.Media;

namespace Client_tests
{
    public class PlayerTests
    {
        [Fact]
        public void CheckCollision_ShouldReturnTrue_ForCollidingRectangles()
        {
            // Arrange
            double x1 = 0, y1 = 0, width1 = 10, height1 = 10;
            double x2 = 5, y2 = 5, width2 = 10, height2 = 10;

            // Act
            bool result = MainWindow.CheckCollision(new Coordinates(x1, y1), width1, height1, new Coordinates(x2, y2), width2, height2);

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
            bool result = MainWindow.CheckCollision(new Coordinates(x1, y1), width1, height1, new Coordinates(x2, y2), width2, height2);

            // Assert
            Assert.False(result);
        }

        //player touches map object
        [Fact]
        public void Test_Player_Touches_Map_Object_Returns_True()
        {
            Coin coin = new Coin(1, 5, 5, 10, "image");
            Player player = new Player(1, "petras", "red", "red", "round");
            player.SetCurrentPosition(5, 5);
            int playerSize = 50;
            bool result = MainWindow.PlayerTouchesMapObject(player.GetCurrentX(), player.GetCurrentY(), playerSize, coin.X, coin.Y, coin.Height);

            Assert.True(result);
        }
        //player doesnt touch map object
        [Fact]
        public void Test_Player_Not_Touches_Map_Object_Returns_False()
        {
            Coin coin = new Coin(1, 5, 5, 10, "image");
            Player player = new Player(1, "petras", "red", "red", "round");
            player.SetCurrentPosition(500, 500);
            int playerSize = 50;
            bool result = MainWindow.PlayerTouchesMapObject(player.GetCurrentX(), player.GetCurrentY(), playerSize, coin.X, coin.Y, coin.Height);

            Assert.False(result);
        }
    }
}