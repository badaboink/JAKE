using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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
        private int _speed;
        private string? _shotColor;
        private string? _shotShape;
        private bool _isShooting = false;
        private double _attackSpeed;
        private string _lastObjectPicked;
        public string? Ability { get; set; }
        

        public Player(int id, string name, string color, string shotColor, string shotShape)
        {
            _id = id;
            _name = name;
            _color = color;
            _attackSpeed = 5;
            _speed = 10;
            _shotColor = shotColor;
            _shotShape = shotShape;
            SetCurrentPosition(0, 0);
             
        }
           
        public Player()
        {
            _id = -1;
            _name = "";
            _color = "red";
            _speed = 10;
            _attackSpeed = 5;
        }

        public int GetId()
        {
            return _id;

        }
        [ExcludeFromCodeCoverage]
        public void SetId(int id)
        {
            this._id =  id;
        }

        public string GetName()
        {
            return _name;
        }
        [ExcludeFromCodeCoverage]
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

        [ExcludeFromCodeCoverage]
        public Coordinates GetCurrentCoords()
        {
            return _currentCoords;
        }
        [ExcludeFromCodeCoverage]
        public Coordinates GetNextCoords(double stepSize)
        {

            double newX = _currentCoords.x + _currentDirection.x * stepSize;
            double newY = _currentCoords.y + _currentDirection.y * stepSize;
            return new Coordinates(newX, newY);
        }
        public string GetLastObjectPicked()
        {
            return _lastObjectPicked;
        }
        public void SetLastObjectPicked(string obj)
        {
            _lastObjectPicked = obj;
        }

        public void SetCurrentPosition(double x, double y)
        {
            _currentCoords.x = x;
            _currentCoords.y = y;
        }
        [ExcludeFromCodeCoverage]
        public void SetCurrentPosition(Coordinates coords)
        {
            this.SetCurrentPosition(coords.x, coords.y);
        }

        [ExcludeFromCodeCoverage]
        public double GetDirectionX()
        {
            return _currentDirection.x;
        }
        [ExcludeFromCodeCoverage]
        public double GetDirectionY()
        {
            return _currentDirection.y;
        }
        [ExcludeFromCodeCoverage]
        public Coordinates GetDirectionCoords()
        {
            return _currentDirection;
        }

        public string? GetShotColor()
        {
            return _shotColor;
        }

        public string? GetShotShape()
        {
            return _shotShape;
        }
        [ExcludeFromCodeCoverage]
        public void SetCurrentDirection(double x, double y)
        {
            _currentDirection.x = x;
            _currentDirection.y = y;
        }
        [ExcludeFromCodeCoverage]
        public int GetSpeed()
        {
            return _speed;
        }

        public string? GetConnectionId()
        {
            return _connectionid;
        }

        public void SetConnectionId(string id)
        {
            _connectionid = id;
        }
        [ExcludeFromCodeCoverage]
        public void SetColor(string color)
        {
            _color = color;
        }
        [ExcludeFromCodeCoverage]
        public void SetSpeed(int speed )
        {
            _speed = speed;
        }
        [ExcludeFromCodeCoverage]
        public void SetShooting(bool shoot)
        {
            _isShooting = shoot;
        }
        [ExcludeFromCodeCoverage]
        public bool IsShooting
        {
            get{return _isShooting;}
        }
        [ExcludeFromCodeCoverage]
        public void SetAttackSpeed(double speed)
        {
            _attackSpeed = speed;
        }
        [ExcludeFromCodeCoverage]
        public double GetAttackSpeed
        {
            get{return _attackSpeed;}
        }
        [ExcludeFromCodeCoverage]
        public void SetShotColor(string color)
        {
            _shotColor = color;
        }
        [ExcludeFromCodeCoverage]
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
        [ExcludeFromCodeCoverage]
        public bool MatchesId(int id)
        {
            return _id == id;
        }
        [ExcludeFromCodeCoverage]
        public virtual (string text, float health, bool shieldOn) Display(float health, bool shield)
        {
            return (GetLastObjectPicked(), health / 2, shield);
        }
        [ExcludeFromCodeCoverage]
        public override bool Equals(object? obj)
        {
            if (obj is Player otherPlayer)
            {
                return this.GetId() == otherPlayer.GetId();
            }
            return false;
        }
        [ExcludeFromCodeCoverage]
        public override int GetHashCode()
        {
            return this.GetId().GetHashCode();
        }
        public override string ToString()
        {
            return $"{GetId()}:{GetName()}:{GetColor()}:{GetCurrentX()}:{GetCurrentY()}:{GetShotColor()}:{GetShotShape()}";
        }

        [ExcludeFromCodeCoverage]
        public void IncreaseSpeed(int speed)
        {

            _speed += speed;
        }

        [ExcludeFromCodeCoverage]
        public void AddShield(int time)
        {
            Debug.WriteLine("uzdejo shield");
        }
    }
}
