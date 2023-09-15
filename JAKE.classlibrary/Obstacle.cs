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
        public override string ToString()
        {
            return $"{Width}:{Height}:{PositionX}:{PositionY}";
        }
    }
}
