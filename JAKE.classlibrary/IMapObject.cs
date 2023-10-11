﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary
{
    public interface IMapObject
    {
        int id { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public Object Image { get; set; }
        void Interact(Player player);
        bool MatchesId(int id);
        public void SetPosition(double x, double y);
    }
}