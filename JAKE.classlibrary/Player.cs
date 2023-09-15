using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary
{
    public class Player
    {
        private int _id;
        private string _name;
        private string _color;
        private int _currentX;
        private int _currentY;
        public string Ability { get; set; }

        public Player(int id, string name, string color)
        {
            _id = id;
            _name = name;
            _color = color;
            SetCurrentPosition(0, 0);
            if (ColorToAbilityMap.TryGetValue(color, out var ability))
            {
                Ability = ability;
            }
            else
            {
                Ability = "unknown";
            }
        }

        public int GetId()
        {
            return _id;
        }

        public string GetName()
        {
            return _name;
        }

        public string GetColor()
        {
            return _color;
        }

        public int GetCurrentX()
        {
            return _currentX;
        }

        public int GetCurrentY()
        {
            return _currentY;
        }

        public void SetCurrentPosition(int x, int y)
        {
            _currentX = x;
            _currentY = y;
        }
        private static readonly Dictionary<string, string> ColorToAbilityMap = new Dictionary<string, string>
        {
            { "Green", "heal" },
            { "Blue", "wall" },
            { "Red", "strength" },
        };
        public override bool Equals(object obj)
        {
            if (obj is Player otherPlayer)
            {
                return this.GetId() == otherPlayer.GetId();
            }
            return false;
        }
        public override int GetHashCode()
        {
            return this.GetId().GetHashCode();
        }
        public override string ToString()
        {
            return $"{GetId()}:{GetName()}:{GetColor()}:{GetCurrentX()}:{GetCurrentY()}";
        }
    }
}
