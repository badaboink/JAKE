using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JAKE.classlibrary.Patterns;

namespace JAKE.classlibrary.Collectibles
{
    [ExcludeFromCodeCoverage]
    public class Weapon : IMapObject
    {
        public int Time { get;  set; }
        public int ShootingSpeed { get;  set; }
        public int ShootingStrength { get;  set; }
        public int id { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Image { get; set; }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public Weapon(int time, int speed, int strength, int width = 20, int heught = 20)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            Time = time;
            ShootingSpeed = speed;
            ShootingStrength = strength;
            Width = width;
            Height = heught;
        }
        public void Interact(GameStats gameStats)
        {

        }
        public bool MatchesId(int id)
        {
            return this.id == id;
        }
        public void SetPosition(double x, double y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return $"{id}:{X}:{Y}:{Width}:{Height}:{ShootingSpeed}:{ShootingStrength}:{Time}";
        }
    }
}
