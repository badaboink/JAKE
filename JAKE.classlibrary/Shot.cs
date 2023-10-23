using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JAKE.classlibrary.Patterns;

namespace JAKE.classlibrary
{
    public class Shot
    {
        private double _speed;
        private int _size;
        private double _points;
        private double _x;
        private double _y;
        private IColor _color;
        private IShape _shape;
        public Shot(IColor color, IShape shape, double speed, int size, double points)
        {         
            _color = color;
            _shape = shape;
            _speed = speed;
            _size = size;
            _points = points;
        }
        public string getShape()
        {
            return _shape.GetShape();
        }
        public string getColor()
        {
            return _color.GetColor();
        }
        public double getSpeed()
        {
            return _speed;
        }
        public int getSize()
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
        public void setColor(IColor color) 
        {
            _color = color;
        }
        public void setShape(IShape shape)
        {
            _shape = shape;
        }
        public void setSize(int size) 
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
