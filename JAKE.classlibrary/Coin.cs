using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JAKE.classlibrary.Patterns;

namespace JAKE.classlibrary
{
    public class Coin : IMapObject
    {

        public int Points { get; private set; }
        public int id { get;  set; }
        public double X { get; set; }
        public double Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Image { get; set; }
        public Coin(int id, int width=7, int height = 7)
        {
            this.id = id;
            Width = width;
            Height = height;
        }

        public Coin(int id)
        {
            this.id = id;
        }
        public void Interact(Player player)
        {
            // Implement coin logic
            //player.IncreaseCoins(1);
        }
        public bool MatchesId(int id)
        {
            return this.id == id;
        }
        public  void SetPosition(double x, double y)
        {
            X = x;
            Y = y;
        }
        public override bool Equals(object obj)
        {
            if (obj is Coin otherCoin)
            {
                return this.MatchesId(otherCoin.id);
            }
            return false;
        }

        public void SetPoints(int points)
        {
            this.Points = points;
        }
        public override string ToString()
        {
            return $"{id}:{X}:{Y}:{Width}:{Height}:{Points}";
        }
    }
}
