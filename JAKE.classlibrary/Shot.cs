using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary
{
    public class Shot
    {
        private double _speed;
        private string _color;
        private double _size;
        private double _points;
        private double _x;
        private double _y;
        public Shot()
        {
            //_speed = speed;
            //_color = color;
            //_size = size;
            //_points = points;
        }
        public double getSpeed()
        {
            return _speed;
        }
        public string getColor() 
        { 
            return _color;
        }
        public double getSize()
        { 
            return _size;
        }
        public double getPoints() 
        { 
            return _points;
        }
        public double getX() 
        { 
            return _x;
        }
        public double getY() 
        {
            return _y;
        }

        public void setSpeed(double speed)
        { 
            _speed = speed; 
        }
        public void setColor(string color) 
        { 
            _color = color;
        }
        public void setSize(double size) 
        {
            _size = size;
        }
        public void setPoints(double points) 
        {
            _points = points;
        }
        public void setPosition(double x, double y) 
        { 
            _x = x;
            _y = y;
        }
        public override string ToString()
        {
            return $"{getX()}:{getY()}";
        }


    }
}
