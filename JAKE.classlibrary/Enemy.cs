using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using JAKE.classlibrary.Patterns;

namespace JAKE.classlibrary
{
    public class Enemy
    {
        private int _id;
        private double _speed;
        private string _color;
        private double _currentX;
        private double _currentY;
        private int _health;
        private int _size;
        private IMoveStrategy movementStrategy;

        public Enemy(int id, string color, double speed = 2, int health = 20, int size=20)
        {
            _id = id;
            _color = color;
            _speed = speed;
            _health = health;
            _size = size;
            SetCurrentPosition(0, 0);
        }

        public int GetId()
        {
            return _id;
        }

        public string GetColor()
        {
            return _color;
        }

        public double GetCurrentX()
        {
            return _currentX;
        }

        public double GetCurrentY()
        {
            return _currentY;
        }

        public double GetSpeed()
        {
            return _speed;
        }

        public void SetCurrentPosition(double x, double y)
        {
            _currentX = x;
            _currentY = y;
        }

        public int GetHealth()
        {
            return _health;
        }

        public void SetHealth(int health)
        {
            _health = health;
        }
        public int GetSize()
        {
            return _size;
        }

        public void SetSize(int size)
        {
            _size = size;
        }
        public void SetSpeed(double speed)
        {
            _speed = speed;
        }
        public IMoveStrategy GetCurrentMovementStrategy()
        {
            return movementStrategy;
        }
        public void SetMovementStrategy(IMoveStrategy movementStrategy)
        {
            this.movementStrategy = movementStrategy;
        }
        public void Move(List<Player> players)
        {
            // Delegate the movement behavior to the current strategy
            if (movementStrategy != null)
            {
                movementStrategy.Move(this, players); // Pass the current enemy object to the strategy
            }
        }
        //private static readonly Dictionary<string, string> ColorToAbilityMap = new Dictionary<string, string>
        //{
        //    { "Green", "heal" },
        //    { "Blue", "wall" },
        //    { "Red", "strength" },
        //};
        public override bool Equals(object obj)
        {
            if (obj is Enemy otherPlayer)
            {
                return this.GetId() == otherPlayer.GetId();
            }
            return false;
        }
        public bool MatchesId(int id)
        {
            return _id == id;
        }
        public Player FindClosestPlayer(List<Player> players)
        {
            Player closestPlayer = null;
            double closestDistance = double.MaxValue;

            foreach (var player in players)
            {
                // Calculate the distance between enemy and player
                double distance = Math.Sqrt(
                    Math.Pow(player.GetCurrentX() - GetCurrentX(), 2) +
                    Math.Pow(player.GetCurrentY() - GetCurrentY(), 2));

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPlayer = player;
                }
            }

            return closestPlayer;
        }
        public override int GetHashCode()
        {
            return this.GetId().GetHashCode();
        }
        public override string ToString()
        {
            return $"{GetId()}:{GetColor()}:{GetCurrentX()}:{GetCurrentY()}:{GetHealth()}:{GetSize()}";
        }
    }
}
