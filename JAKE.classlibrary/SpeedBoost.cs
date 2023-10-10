using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary
{
    public class SpeedBoost : IMapObject
    {
        public int Value { get; private set; }
        public int id { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Object Image { get; set; }
        public SpeedBoost(int value, int width = 7, int heught = 7)
        {
            Value = value;
            Width = width;
            Height = heught;
        }

        public void Interact(Player player)
        {
            // Implement speed boost logic
            // player.IncreaseSpeed(2);
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
    }
}
