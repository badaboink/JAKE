using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
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

        }
        public virtual double getSpeed()
        {
            return _speed;
        }
        public virtual string getColor() 
        { 
            return _color;
        }
        public virtual double getSize()
        { 
            return _size;
        }
        public virtual double getPoints() 
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
        public void setShot(string color, double speed, double size, double points)
        {
            setColor(color);
            setSpeed(speed);
            setSize(size);
            setPoints(points);
        }
        public override string ToString()
        {
            return $"{getX()}:{getY()}";
        }
    }
}
