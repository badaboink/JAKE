using JAKE.classlibrary.Patterns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary.Collectibles
{
    public class Corona : IMapObject
    {
        public int id { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Image { get; set; }
        public Corona(int id, double x, double y, string image)
        {
            this.id = id;
            X = x;
            Y = y;
            Width = 20;
            Height = 20;
            Image = image;
        }

        public Corona(int id, double x, double y)
        {
            this.id = id;
            X = x;
            Y = y;
            Width = 20;
            Height = 20;
        }

        public Corona()
        {
            Width = 20;
            Height = 20;
        }

        public void Interact(GameStats gameStats)
        {
            gameStats.state = "corona";
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
        public override bool Equals(object obj)
        {
            if (obj is Corona otherCorona)
            {
                return this.MatchesId(otherCorona.id);
            }
            return false;
        }

        public override string ToString()
        {
            return $"{id}:{X}:{Y}:{Width}:{Height}";
        }
        
    }
}
