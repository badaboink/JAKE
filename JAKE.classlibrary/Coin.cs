﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public Object Image { get; set; }
        public Coin(int points, int width=7, int heught = 7)
        {
            Points = points;
            Width = width;
            Height = heught;
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

        public override string ToString()
        {
            return $"{id}:{X}:{Y}:{Width}:{Height}:{Points}";
        }
    }
}