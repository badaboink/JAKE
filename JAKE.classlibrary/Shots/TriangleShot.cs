﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JAKE.classlibrary.Patterns;

namespace JAKE.classlibrary.Shots
{
    public class TriangleShot : IShape
    {
        public string GetShape()
        {
            return "triangle";
        }
    }
}
