using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary
{
    public class Obstacle
    {
        public double Width { get; set; }
        public double Height { get; set; }
        public double PositionX { get; set; }
        public double PositionY { get; set; }

        public Obstacle(double width, double height, double positionX, double positionY)
        {
            Width = width;
            Height = height;
            PositionX = positionX;
            PositionY = positionY;
        }

        public bool WouldOverlap(double newX, double newY, double playerWidth, double playerHeight)
        {
            // Calculate the boundaries of the obstacle
            double obstacleLeft = PositionX;
            double obstacleRight = PositionX + Width;
            double obstacleTop = PositionY;
            double obstacleBottom = PositionY + Height;

            // Calculate the boundaries of the new position
            double newLeft = newX;
            double newRight = newX + playerWidth;
            double newTop = newY;
            double newBottom = newY + playerHeight;

            // Check for overlap
            bool overlapX = newLeft < obstacleRight && newRight > obstacleLeft;
            bool overlapY = newTop < obstacleBottom && newBottom > obstacleTop;

            return overlapX && overlapY;
        }

        public double DistanceFromObstacle(int x, int y, double playerX, double playerY, double playerWidth, double playerHeight)
        {
            double distance = 0;
            if(x!=0)
            {
                double Distance_X_Left = x == -1 ? PositionX + Width - playerX : double.MaxValue;
                double Distance_X_Right = x == 1 ? playerX + playerWidth - PositionX : double.MaxValue;
                distance = Distance_X_Left != double.MaxValue ? Distance_X_Left : Distance_X_Right;
            }
            else
            {
                double Distance_Y_Top = y == -1 ? PositionY + Height - playerY : double.MaxValue;
                double Distance_Y_Bottom = y == 1 ? playerY + playerHeight - PositionY : double.MaxValue;
                distance = Distance_Y_Top != double.MaxValue ? Distance_Y_Top : Distance_Y_Bottom;
            }
            return distance;
        }
        public override string ToString()
        {
            return $"{Width}:{Height}:{PositionX}:{PositionY}";
        }
    }
}
