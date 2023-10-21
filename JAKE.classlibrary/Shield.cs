using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JAKE.classlibrary.Patterns;

namespace JAKE.classlibrary
{
    public class Shield : IMapObject
    {
        public int Time { get; set; }
        public int id { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Image { get; set; }
        public Shield(int id, double x, double y, int time, string image)
        {
            Time = time;
            Width = 20;
            Height = 20;
            this.id = id;
            this.X = x;
            this.Y = y;
            Image = image;
        }

        public Shield(int id, double x, double y, int time)
        {
            Time = time;
            Width = 20;
            Height = 20;
            this.id = id;
            this.X = x;
            this.Y = y;
        }
        public Shield(int time)
        {
            Time = time;
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
            return $"{id}:{X}:{Y}:{Width}:{Height}:{Time}";
        }

        public void SetTime(int time)
        {
            this.Time = time;
        }

        //public void Interact(GameStats gameStats)
        //{
        //    gameStats.Shield = true;
        //}

        public void Interact(GameStats gameStats)
        {

        }
    }
}
