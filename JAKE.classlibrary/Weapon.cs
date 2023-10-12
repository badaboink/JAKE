using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JAKE.classlibrary.Patterns;

namespace JAKE.classlibrary
{
    public class Weapon : IMapObject
    {
        public int Time { get; private set; }
        public int ShootingSpeed { get; private set; }
        public int ShootingStrength { get; private set; }
        public int id { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Image { get; set; }
        public Weapon(int time, int speed, int strength, int width = 7, int heught = 7)
        {
            Time = time;
            ShootingSpeed = speed;
            ShootingStrength = strength;
            Width = width;
            Height = heught;
        }
        public void Interact(Player player)
        {
            // Implement weapon logic
            //player.EquipWeapon("Sword");
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
