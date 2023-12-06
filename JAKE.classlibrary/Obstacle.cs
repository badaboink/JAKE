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
                return HandleInsideObstacle(position, size, center);
            }
            else
            {
                return HandlePartiallInObstacle(position, size);
            }
        }

        private Coordinates HandlePartiallInObstacle(Coordinates position, Coordinates size)
        {
            CalculateCornersCollisionsAndDistances(position, size, out int[] cornerCollisions, out double[] distances);
            _ = Math.Min(Math.Min(distances[0], distances[2]), Math.Min(distances[3], distances[1]));
            int cornerCount = cornerCollisions.Sum();
            if (cornerCount == 2)
            {
                return HandleSideCollision(position, cornerCollisions, distances);
            }
            if (cornerCount != 1)
            {
                return position;
            }
            if (cornerCollisions[0] == 1)
            {
                return distances[2] < distances[1] ? new Coordinates(position.x, position.y + distances[2]) : new Coordinates(position.x + distances[1], position.y);
            }
            if (cornerCollisions[1] == 1)
            {
                return distances[2] < distances[3] ? new Coordinates(position.x, position.y + distances[2]) : new Coordinates(position.x - distances[3], position.y);
            }
            if (cornerCollisions[3] == 1)
            {
                return distances[0] < distances[1] ? new Coordinates(position.x, position.y - distances[0]) : new Coordinates(position.x + distances[1], position.y);
            }
            if (cornerCollisions[2] == 1)
            {
                return distances[0] < distances[3] ? new Coordinates(position.x, position.y - distances[0]) : new Coordinates(position.x - distances[3], position.y);
            }
            return position;
        }

        private void CalculateCornersCollisionsAndDistances(Coordinates position, Coordinates size, out int[] corners, out double[] distances)
        {
            int topLeft, topRight, bottomRight, bottomLeft;
            double distanceTop, distanceRight, distanceBottom, distanceLeft;
            topLeft = WouldOverlap(position.x, position.y) ? 1 : 0;
            topRight = WouldOverlap(position.x + size.x, position.y) ? 1 : 0;
            bottomRight = WouldOverlap(position.x + size.x, position.y + size.y) ? 1 : 0;
            bottomLeft = WouldOverlap(position.x, position.y + size.y) ? 1 : 0;
            distanceTop = Math.Abs(this.PositionY - (position.y + size.y));
            distanceRight = Math.Abs(this.PositionX + this.Width - position.x);
            distanceBottom = Math.Abs(this.PositionY + this.Height - position.y);
            distanceLeft = Math.Abs(this.PositionX - (position.x + size.x));
            distances = new double[] { distanceTop, distanceRight, distanceBottom, distanceLeft };
            corners = new int[] { topLeft, topRight, bottomRight, bottomLeft };
        }

        private Coordinates HandleInsideObstacle(Coordinates position, Coordinates size, Coordinates center)
        {
            double distanceTop = Math.Abs(this.PositionY - center.y);
            double distanceRight = Math.Abs(this.PositionX + this.Width - center.x);
            double distanceBottom = Math.Abs(this.PositionY + this.Height - center.y);
            double distanceLeft = Math.Abs(this.PositionX - center.x);
            double minDistance = Math.Min(Math.Min(distanceTop, distanceBottom), Math.Min(distanceLeft, distanceRight));
            if (distanceTop <= minDistance)
            {
                return new Coordinates(position.x, this.PositionY - size.y);
            }
            else if (distanceBottom <= minDistance)
            {
                return new Coordinates(position.x, this.PositionY + this.Height);
            }
            else if (distanceLeft <= minDistance)
            {
                return new Coordinates(this.PositionX - size.x, position.y);
            }
            else if (distanceRight <= minDistance)
            {
                return new Coordinates(this.PositionX + this.Width, position.y);
            }
            else
            {
                return position;
            }
        }

        private static Coordinates HandleSideCollision(Coordinates position, int[] corners, double[] distances)
        {
            int top = corners[0] + corners[1];
            int bottom = corners[3] + corners[2];
            int right = corners[1] + corners[2];
            int left = corners[0] + corners[3];
            if (top == 2)
            {
                return new Coordinates(position.x, position.y + distances[2]);
            }
            if (bottom == 2)
            {
                return new Coordinates(position.x, position.y - distances[0]);
            }
            if (left == 2)
            {
                return new Coordinates(position.x + distances[1], position.y);
            }
            if (right == 2)
            {
                return new Coordinates(position.x - distances[3], position.y);
            }
            return position;
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
