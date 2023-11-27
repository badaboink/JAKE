using JAKE.classlibrary.Patterns;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

        public bool WouldOverlap(double newX, double newY)
        {
            // Calculate the boundaries of the obstacle
            double obstacleLeft = PositionX;
            double obstacleRight = PositionX + Width;
            double obstacleTop = PositionY;
            double obstacleBottom = PositionY + Height;

            // Check for overlap
            bool overlapX = newX < obstacleRight && newX > obstacleLeft;
            bool overlapY = newY < obstacleBottom && newY > obstacleTop;

            return overlapX && overlapY;
        }

        public double DistanceFromObstacle(int x, int y, double playerX, double playerY, double playerWidth, double playerHeight)
        {
            double distance;
            if (x!=0)
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

        public Coordinates MoveToClosestEdge(Coordinates position, double width, double height)
        {
            return MoveToClosestEdge(position, new Coordinates(width, height));

        }
        public Coordinates MoveToClosestEdge(Coordinates position, Coordinates size)
        {
            
            Coordinates center = new(position.x + size.x/2, position.y + size.y/2);
            
            if (WouldOverlap(center.x, center.y))
            {
                double distanceTop = Math.Abs(this.PositionY - center.y);
                double distanceRight = Math.Abs(this.PositionX + this.Width - center.x);
                double distanceBottom = Math.Abs(this.PositionY + this.Height - center.y);
                double distanceLeft = Math.Abs(this.PositionX - center.x);
                double minDistance = Math.Min(Math.Min(distanceTop, distanceBottom), Math.Min(distanceLeft, distanceRight));
                if(distanceTop <= minDistance)
                {
                    return new Coordinates(position.x, this.PositionY - size.y);
                }
                else if(distanceBottom <= minDistance)
                {
                    return new Coordinates(position.x, this.PositionY + this.Height);
                }
                else if(distanceLeft <= minDistance)
                {
                    return new Coordinates(this.PositionX - size.x, position.y);
                }
                else if(distanceRight <= minDistance)
                {
                    return new Coordinates(this.PositionX + this.Width, position.y);
                }
                else
                {
                    return position;
                }
            }
            else
            {
                int topLeft = WouldOverlap(position.x, position.y) ? 1 : 0;
                int topRight = WouldOverlap(position.x + size.x, position.y) ? 1 : 0;
                int bottomRight = WouldOverlap(position.x + size.x, position.y + size.y) ? 1 : 0;
                int bottomLeft = WouldOverlap(position.x, position.y + size.y) ? 1 : 0;
                double distanceTop = Math.Abs(this.PositionY - (position.y + size.y));
                double distanceRight = Math.Abs(this.PositionX + this.Width - position.x);
                double distanceBottom = Math.Abs(this.PositionY + this.Height - position.y);
                double distanceLeft = Math.Abs(this.PositionX - (position.x + size.x));
                _ = Math.Min(Math.Min(distanceTop, distanceBottom), Math.Min(distanceLeft, distanceRight));
                int cornerCount = topLeft + topRight + bottomRight + bottomLeft;
                if(cornerCount == 2)
                {
                    int top = topLeft + topRight;
                    int bottom = bottomLeft + bottomRight;
                    int right = topRight + bottomRight;
                    int left = topLeft + bottomLeft;
                    if(top == 2)
                    {
                        return new Coordinates(position.x, position.y + distanceBottom);
                    }
                    if(bottom == 2)
                    {
                        return new Coordinates(position.x, position.y - distanceTop);
                    }
                    if(left == 2)
                    {
                        return new Coordinates(position.x + distanceRight, position.y);
                    }
                    if(right == 2)
                    {
                        return new Coordinates(position.x - distanceLeft, position.y);
                    }
                    return position;
                }
                if(cornerCount == 1)
                {
                    if(topLeft == 1)
                    {
                        return distanceBottom < distanceRight ? new Coordinates(position.x, position.y + distanceBottom) : new Coordinates(position.x + distanceRight, position.y);
                    }
                    if(topRight == 1)
                    {
                        return distanceBottom < distanceLeft ? new Coordinates(position.x, position.y + distanceBottom) : new Coordinates(position.x - distanceLeft, position.y);
                    }
                    if(bottomLeft == 1)
                    {
                        return distanceTop < distanceRight ? new Coordinates(position.x, position.y - distanceTop) : new Coordinates(position.x + distanceRight, position.y);
                    }
                    if(bottomRight == 1)
                    {
                        return distanceTop < distanceLeft ? new Coordinates(position.x, position.y - distanceTop) : new Coordinates(position.x - distanceLeft, position.y);
                    }
                }
                return position;
            }
        }

        public double DistanceFromObstacleX(double speed, double playerDirectionX, double playerCurrentX, double playerWidth)
        {
            double obstacleLeft = this.PositionX;
            double obstacleRight = this.PositionX + this.Width;

            double playerLeft = playerCurrentX;
            double playerRight = playerCurrentX + playerWidth;

            if (playerDirectionX > 0)
            {
                // Player is moving right
                double distanceToObstacle = obstacleLeft - playerRight;
                return distanceToObstacle > playerDirectionX * speed ? distanceToObstacle : 0;
            }
            else if (playerDirectionX < 0)
            {
                // Player is moving left
                double distanceToObstacle = playerLeft - obstacleRight;
                return distanceToObstacle > playerDirectionX * speed ? distanceToObstacle : 0;
            }
            else
            {
                // Player is not moving horizontally
                return 0;
            }

        }

        public double DistanceFromObstacleY(double speed, double playerDirectionY, double playerCurrentY, double playerHeight)
        {
            double obstacleTop = this.PositionY;
            double obstacleBottom = this.PositionY + this.Height;

            double playerTop = playerCurrentY;
            double playerBottom = playerCurrentY + playerHeight;

            if (playerDirectionY > 0)
            {
                // Player is moving down
                double distanceToObstacle = obstacleTop - playerBottom;
                return distanceToObstacle < playerDirectionY * speed ? distanceToObstacle : 0;
            }
            else if (playerDirectionY < 0)
            {
                // Player is moving up
                double distanceToObstacle = playerTop - obstacleBottom;
                return distanceToObstacle > playerDirectionY * speed ? distanceToObstacle : 0;
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

        [ExcludeFromCodeCoverage]
        public override string ToString()
        {
            return $"{Width}:{Height}:{PositionX}:{PositionY}";
        }
    }
}
