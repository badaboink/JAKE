using JAKE.classlibrary.Patterns;
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
                double Distance_X_Left = x == -1 ? (playerX - Width - PositionX)*-1 : double.MaxValue;
                double Distance_X_Right = x == 1 ? PositionX - playerWidth - playerX : double.MaxValue;
                distance = Distance_X_Left != double.MaxValue ? Distance_X_Left : Distance_X_Right;
            }
            else
            {
                double Distance_Y_Top = y == -1 ? (playerY - PositionY - Height) *-1 : double.MaxValue;
                double Distance_Y_Bottom = y == 1 ? PositionY - playerY - playerHeight : double.MaxValue;
                distance = Distance_Y_Top != double.MaxValue ? Distance_Y_Top : Distance_Y_Bottom;
            }
            return distance;
        }

        public double DistanceFromObstacleX(GameStats gameStat, double playerDirectionX, double playerCurrentX, double playerWidth)
        {
            double obstacleLeft = this.PositionX;
            double obstacleRight = this.PositionX + this.Width;

            double playerLeft = playerCurrentX;
            double playerRight = playerCurrentX + playerWidth;

            if (playerDirectionX > 0)
            {
                // Player is moving right
                double distanceToObstacle = obstacleLeft - playerRight;
                return distanceToObstacle < gameStat.PlayerSpeed ? distanceToObstacle : 0;
            }
            else if (playerDirectionX < 0)
            {
                // Player is moving left
                double distanceToObstacle = playerLeft - obstacleRight;
                return distanceToObstacle > -gameStat.PlayerSpeed ? distanceToObstacle : 0;
            }
            else
            {
                // Player is not moving horizontally
                return 0;
            }
        }

        public double DistanceFromObstacleY(GameStats gameStat, double playerDirectionY, double playerCurrentY, double playerHeight)
        {
            double obstacleTop = this.PositionY;
            double obstacleBottom = this.PositionY + this.Height;

            double playerTop = playerCurrentY;
            double playerBottom = playerCurrentY + playerHeight;

            if (playerDirectionY > 0)
            {
                // Player is moving down
                double distanceToObstacle = obstacleTop - playerBottom;
                return distanceToObstacle < gameStat.PlayerSpeed ? distanceToObstacle : 0;
            }
            else if (playerDirectionY < 0)
            {
                // Player is moving up
                double distanceToObstacle = playerTop - obstacleBottom;
                return distanceToObstacle > -gameStat.PlayerSpeed ? distanceToObstacle : 0;
            }
            else
            {
                // Player is not moving vertically
                return 0;
            }
        }
        public double DistanceFromShieldX(GameStats gameStat, double playerDirectionX, double playerCurrentX, double playerWidth)
        {
            double shieldLeft = this.PositionX;
            double shieldRight = this.PositionX + this.Width;

            double playerLeft = playerCurrentX;
            double playerRight = playerCurrentX + playerWidth;

            if (playerDirectionX > 0)
            {
                // Player is moving right
                double distanceToShield = shieldLeft - playerRight;
                return distanceToShield < gameStat.PlayerSpeed ? distanceToShield : 0;
            }
            else if (playerDirectionX < 0)
            {
                // Player is moving left
                double distanceToShield = playerLeft - shieldRight;
                return distanceToShield > -gameStat.PlayerSpeed ? distanceToShield : 0;
            }
            else
            {
                // Player is not moving horizontally
                return 0;
            }
        }

        public double DistanceFromShieldY(GameStats gameStat, double playerDirectionY, double playerCurrentY, double playerHeight)
        {
            double shieldTop = this.PositionY;
            double shieldBottom = this.PositionY + this.Height;

            double playerTop = playerCurrentY;
            double playerBottom = playerCurrentY + playerHeight;

            if (playerDirectionY > 0)
            {
                // Player is moving down
                double distanceToShield = shieldTop - playerBottom;
                return distanceToShield < gameStat.PlayerSpeed ? distanceToShield : 0;
            }
            else if (playerDirectionY < 0)
            {
                // Player is moving up
                double distanceToShield = playerTop - shieldBottom;
                return distanceToShield > -gameStat.PlayerSpeed ? distanceToShield : 0;
            }
            else
            {
                // Player is not moving vertically
                return 0;
            }
        }


        public override string ToString()
        {
            return $"{Width}:{Height}:{PositionX}:{PositionY}";
        }
    }
}
