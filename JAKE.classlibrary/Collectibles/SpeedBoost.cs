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
    public class SpeedBoost : IMapObject
    {
        public int Speed { get; set; }
        public int id { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Time { get; set; }
        public string Image { get; set; }
        public SpeedBoost(int id, double x, double y, int speed, string image)
        {
            Speed = speed;
            Width = 20;
            Height = 20;
            Time = 15;
            Speed = speed;
            Image = image;
            this.id = id;
            X = x;
            Y = y;
        }
        public SpeedBoost(int speed)
        {
            Speed = speed;
        }
        
        public void Interact(GameStats gameStats)
        {
            gameStats.PlayerSpeed += this.Speed;
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
            return $"{id}:{X}:{Y}:{Width}:{Height}:{Speed}:{Time}";
        }
      
    }
}
