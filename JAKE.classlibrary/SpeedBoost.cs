using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary
{
    public class SpeedBoost : IMapObject
    {
        public int Speed { get; private set; }
        public int id { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Time { get; set; }
        public string Image { get; set; }
        public SpeedBoost(int speed, int width = 7, int heught = 7)
        {
            Speed = speed;
            Width = width;
            Height = heught;
            Time = 15;
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

        public override string ToString()
        {
            return $"{id}:{X}:{Y}:{Width}:{Height}:{Speed}:{Time}";
        }
        public void SetSpeedTime(int speed, int time)
        {
            this.Speed = speed;
            this.Time = time;
        }
    }
}
