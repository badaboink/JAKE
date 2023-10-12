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
        public int Time { get; private set; }
        public int id { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Image { get; set; }
        public Shield(int time, int width = 7, int heught = 7)
        {
            Time = time;
            Width = width;
            Height = heught;
        }

        public void Interact(Player player)
        {
            // Implement shield logic
           // player.EquipShield("Wooden Shield");
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
    }
}
