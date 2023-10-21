using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary.Patterns
{
    public interface IMapObject
    {
        int id { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public string Image { get; set; }
        void Interact(GameStats gamestats);
        bool MatchesId(int id);
        public void SetPosition(double x, double y);
        public string ToString();
    }
}
