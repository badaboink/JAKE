﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JAKE.classlibrary.Patterns;

namespace JAKE.classlibrary.Shots
{
    public class Shot
    {
        private double _speed;
        private int _size;
        private double _points;
        private double _x;
        private double _y;
        private DateTime _previousUpdate;
        private IColor _color;
        private string _shape;
        private Shot shot;

        public Shot(IColor color, double speed, int size, double points)
        {         
            _color = color;
            _speed = speed;
            _size = size;
            _points = points;
            _previousUpdate = DateTime.Now;
        }

        public Shot(Shot shot)
        {
            _color = shot.getColor();
            _speed = shot.getSpeed();
            _size = shot.getSize();
            _points = shot.getPoints();
            _previousUpdate = DateTime.Now;
        }

        public string getShape()
        {
            return _shape;
        }
        public IColor getColor()
        {
            return _color;
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
        [ExcludeFromCodeCoverage]
        public void setSpeed(double speed)
        { 
            _speed = speed; 
        }
        [ExcludeFromCodeCoverage]
        public void setColor(IColor color) 
        {
            _color = color;
        }
        [ExcludeFromCodeCoverage]
        public void setShape(string shape)
        {
            _shape = shape;
        }
        [ExcludeFromCodeCoverage]
        public void setSize(int size) 
        {
            _size = size;
        }
        [ExcludeFromCodeCoverage]
        public void setPoints(double points) 
        {
            _points = points;
        }
        [ExcludeFromCodeCoverage]
        public void setPosition(double x, double y) 
        { 
            _x = x;
            _y = y;
        }
        [ExcludeFromCodeCoverage]
        public override string ToString()
        {
            return $"{getX()}:{getY()}";
        }
        [ExcludeFromCodeCoverage]
        public void SetPreviousUpdate(DateTime time)
        {
            _previousUpdate = time;
        }
        [ExcludeFromCodeCoverage]
        public double DeltaTime
        {
            get
            {
                double delta = DateTime.Now.Subtract(_previousUpdate).TotalMilliseconds;
                _previousUpdate = DateTime.Now;
                return delta/10f;
            }
        }
    }
}
