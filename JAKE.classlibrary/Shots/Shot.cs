using System;
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


#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public Shot(IColor color, double speed, int size, double points)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {         
            _color = color;
            _speed = speed;
            _size = size;
            _points = points;
            _previousUpdate = DateTime.UtcNow;
        }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public Shot(Shot shot)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            _color = shot.GetColor();
            _speed = shot.GetSpeed();
            _size = shot.GetSize();
            _points = shot.GetPoints();
            _previousUpdate = DateTime.UtcNow;
        }

        public string GetShape()
        {
            return _shape;
        }
        public IColor GetColor()
        {
            return _color;
        }
        public double GetSpeed()
        {
            return _speed;
        }
        public int GetSize()
        { 
            return _size;
        }
        public double GetPoints() 
        { 
            return _points;
        }

        public double GetX() 
        { 
            return _x;
        }
        public double GetY() 
        {
            return _y;
        }
        [ExcludeFromCodeCoverage]
        public void SetSpeed(double speed)
        { 
            _speed = speed; 
        }
        [ExcludeFromCodeCoverage]
        public void SetColor(IColor color) 
        {
            _color = color;
        }
        [ExcludeFromCodeCoverage]
        public void SetShape(string shape)
        {
            _shape = shape;
        }
        [ExcludeFromCodeCoverage]
        public void SetSize(int size) 
        {
            _size = size;
        }
        [ExcludeFromCodeCoverage]
        public void SetPoints(double points) 
        {
            _points = points;
        }
        [ExcludeFromCodeCoverage]
        public void SetPosition(double x, double y) 
        { 
            _x = x;
            _y = y;
        }
        [ExcludeFromCodeCoverage]
        public override string ToString()
        {
            return $"{GetX()}:{GetY()}";
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
                double delta = DateTime.UtcNow.Subtract(_previousUpdate).TotalMilliseconds;
                _previousUpdate = DateTime.UtcNow;
                return delta/10f;
            }
        }
    }
}
