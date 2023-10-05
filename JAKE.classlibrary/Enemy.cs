using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

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
            return _health;
        }

        public void SetSize(int size)
        {
            _size = size;
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
        public override int GetHashCode()
        {
            return this.GetId().GetHashCode();
        }
        public override string ToString()
        {
            return $"{GetId()}:{GetColor()}:{GetCurrentX()}:{GetCurrentY()}:{GetHealth()}";
        }
    }
}
