using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace JAKE.classlibrary
{
    public class Player
    {
        private string? _connectionid;
        private int _id;
        private string _name;
        private string _color;
        private double _currentX;
        private double _currentY;
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
        
        
        public Player()
        {
            _id = -1;
            _name = "";
            _color = "red";
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

        public double GetCurrentX()
        {
            return _currentX;
        }

        public double GetCurrentY()
        {
            return _currentY;
        }
        public string GetConnectionId()
        {
            return _connectionid;
        }

        public void SetCurrentPosition(double x, double y)
        {
            _currentX = x;
            _currentY = y;
        }
        public void SetConnectionId(string id)
        {
            _connectionid = id;
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
