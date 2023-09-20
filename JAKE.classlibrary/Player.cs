using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary
{
    public class Player : INotifyPropertyChanged
    {
        private int _id;
        private string _name;
        private string _color;
        private double _currentX;
        private double _currentY;
        private int _score = 0; // Initialize score to zero

        public int Score
        {
            get { return _score; }
            set
            {
                _score = value;
                OnPropertyChanged(nameof(Score));
            }
        }
        private int _health = 100;

        public int Health
        {
            get { return _health; }
            set
            {
                _health = value;
                OnPropertyChanged(nameof(Health));
            }
        }
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
            _score = 0;
            _health = 100;
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

        public int GetHealth()
        {
            return _health;
        }

        public void SetScore(int score)
        {
            _score = score;
        }

        public void SetHealth(int health)
        {
            _health = health;
        }

        public void SetCurrentPosition(double x, double y)
        {
            _currentX = x;
            _currentY = y;
        }
        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
