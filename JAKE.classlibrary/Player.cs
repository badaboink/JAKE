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
        private Coordinates _currentDirection = new(0, 1);
        private Coordinates _currentCoords = new(0, 0);
        private Stack<Coordinates> _history = new Stack<Coordinates>();
        private int _speed;
        private string _shotColor;
        private string _shotShape;
        public string Ability { get; set; }
        private class Coordinates
        {
            public double x;
            public double y;

            public Coordinates()
            {
            }

            public Coordinates(double x, double y)
            {
                this.x = x;
                this.y = y;
            }


        }

        public Player(int id, string name, string color, string shotColor, string shotShape)
        {
            _id = id;
            _name = name;
            _color = color;
            _speed = 10;
            _shotColor = shotColor;
            _shotShape = shotShape;
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
            return _currentCoords.x;
        }
        public double GetCurrentY()
        {
            return _currentCoords.y;
        }

        public void SetCurrentPosition(double x, double y)
        {
            _history.Push(new Coordinates(_currentCoords.x, _currentCoords.y));
            _currentCoords.x = x;
            _currentCoords.y = y;
        }

        public double GetDirectionX()
        {
            return _currentDirection.x;
        }

        public double GetDirectionY()
        {
            return _currentDirection.y;
        }

        public string GetShotColor()
        {
            return _shotColor;
        }

        public string GetShotShape()
        {
            return _shotShape;
        }

        public void SetCurrentDirection(double x, double y)
        {
            _currentDirection.x = x;
            _currentDirection.y = y;
        }
        public int GetSpeed()
        {
            return _speed;
        }

        public string GetConnectionId()
        {
            return _connectionid;
        }

        public void Undo()
        {
            if (_history.Count > 0)
            {
                _currentCoords = _history.Pop();
            }
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
        public void SetShotColor(string color)
        {
            _shotColor = color;
        }
        public void SetShotShape(string shape)
        {
            _shotShape = shape;
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
            return $"{GetId()}:{GetName()}:{GetColor()}:{GetCurrentX()}:{GetCurrentY()}:{GetShotColor()}:{GetShotShape()}";
        }
        // :{GetShotColor()}:{GetShotShape()}
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
