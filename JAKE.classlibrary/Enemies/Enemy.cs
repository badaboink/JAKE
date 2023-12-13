using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using JAKE.classlibrary.Patterns;
using JAKE.classlibrary.Patterns.Flyweight;
using JAKE.classlibrary.Patterns.Strategies;

namespace JAKE.classlibrary.Enemies
{
    public class Enemy
    {
        private int _id;
        private double _speed;
        private string _color;
        private readonly Coordinates coordinates;
        private readonly Trigger trigger = new();
        private int _health;
        private int _size;
#pragma warning disable S2933 // Fields that are only assigned in the constructor should be "readonly"
#pragma warning disable S4487 // Unread "private" fields should be removed
#pragma warning disable IDE0044 // Add readonly modifier
#pragma warning disable IDE0052 // Remove unread private members
        private int _points;
#pragma warning restore IDE0052 // Remove unread private members
#pragma warning restore IDE0044 // Add readonly modifier
#pragma warning restore S4487 // Unread "private" fields should be removed
#pragma warning restore S2933 // Fields that are only assigned in the constructor should be "readonly"
        protected IMoveStrategy? movementStrategy;

        public virtual Enemy? ShallowClone()
        {
            return MemberwiseClone() as Enemy;
        }

        public Enemy(int id, string color, double speed = 2, int health = 20, int size=20, int points = 10)
        {
            _id = id;
            _color = color;
            _speed = speed;
            _health = health;
            _size = size;
            _points = points;
            coordinates = new Coordinates(0, 0);
            SetCurrentPosition(0, 0);
        }

        public bool Trigerred
        {
            get { return trigger.trigger; }
        }

        public void SetId(int id)
        {
            this._id = id;
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
            return coordinates.x;
        }

        public double GetCurrentY()
        {
            return coordinates.y;
        }

        public double GetSpeed()
        {
            return _speed;
        }
        public int GetHealth()
        {
            return _health;
        }
        public int GetSize()
        {
            return _size;
        }

        public void SetCurrentPosition(double x, double y)
        {
            coordinates.x = x;
            coordinates.y = y;
        }

        public void SetHealth(int health)
        {
            _health = health;
        }

        public void SetSize(int size)
        {
            _size = size;
        }
        public void SetColor(string color)
        {
            _color = color;
        }
        public void SetPoints(int points)
        {
            _points = points;
        }
        public void SetSpeed(double speed)
        {
            _speed = speed;
        }
        public IMoveStrategy? GetCurrentMovementStrategy()
        {
            return movementStrategy;
        }
        public void SetMovementStrategy(IMoveStrategy movementStrategy)
        {
            this.movementStrategy = movementStrategy;
        }
        public virtual void Move(List<Player> players)
        {
            if (movementStrategy != null)
            {
                movementStrategy.Move(this, players);
            }
        }
        public override bool Equals(object? obj)
        {
            if (obj is Enemy otherPlayer)
            {
                return this.GetId() == otherPlayer.GetId();
            }
            return false;
        }

        public virtual void Hit()
        {
            trigger.Flip();
        }
        public bool MatchesId(int id)
        {
            return _id == id;
        }
        public Player? FindClosestPlayer(List<Player> players)
        {
            Player? closestPlayer = null;
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
