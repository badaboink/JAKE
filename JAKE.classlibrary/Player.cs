using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private int _speed;
        public string Ability { get; set; }

        public Player(int id, string name, string color)
        {
            _id = id;
            _name = name;
            _color = color;
            _speed = 10;
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
            _speed = 10;
        }

        public int GetId()
        {
            return _id;

        }
        public void SetId(int id)
        {
            this._id =  id;
        }

        public string GetName()
        {
            return _name;
        }

        public void SetName(string name)
        {
            _name = name;
        }

        public string GetColor()
        {
            return _color;
        }

        public double GetCurrentX()
        {
            return _currentX;
        }

        public int GetSpeed()
        {
            return _speed; ;
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
        public void SetColor(string color)
        {
            _color = color;
        }
        public void SetSpeed(int speed )
        {
            _speed = speed;
        }
        private static readonly Dictionary<string, string> ColorToAbilityMap = new Dictionary<string, string>
        {
            { "Green", "heal" },
            { "Blue", "wall" },
            { "Red", "strength" },
        };
        public bool MatchesId(int id)
        {
            return _id == id;
        }
        public virtual (string text, float health, bool shieldOn) Display(string text, float health, bool shieldOn)
        {
            return (text, health, shieldOn);
        }
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

        //public void IncreaseHealth(int health)
        //{
        //    //health padidint 
        //    Debug.WriteLine("padidino health");
        //}

        public void IncreaseSpeed(int speed)
        {
            //speed padidint ir uzdet laika bet ne cia gal?? laikas default 15s
            Debug.WriteLine("padidino speed");
            _speed += speed;
        }

        //public void IncreasePoints(int points)
        //{
        //    //points padidint
        //    Debug.WriteLine("padidino points");
        //}

        public void AddShield(int time)
        {
            //uzdet shield grafika ir laika uzdet??
            Debug.WriteLine("uzdejo shield");
        }
    }
}
