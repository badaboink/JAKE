using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary
{
    public class HealthBoost : IMapObject
    {
        public int Health { get;  set; }

        public int id { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Image { get; set; }
        public HealthBoost(int health, int width = 7, int heught = 7)
        {
            Health = health;
            Width = width;
            Height = heught;
        }
        public void Interact(Player player)
        {
            // Implement health boost logic
            // player.IncreaseHealth(50); //galimai is singleton pasiimt player ir pakeist health
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
            return $"{id}:{X}:{Y}:{Width}:{Height}:{Health}";
        }

        public void SetHealth(int health)
        {
            this.Health=health;
        }
    }
}
