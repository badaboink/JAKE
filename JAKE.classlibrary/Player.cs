using System;
using System.Collections.Generic;
using System.Linq;
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

        public Player(int id, string name, string color)
        {
            _id = id;
            _name = name;
            _color = color;
            SetCurrentPosition(0, 0);
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
    }
}
