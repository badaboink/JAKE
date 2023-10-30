using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JAKE.classlibrary.Patterns;

namespace JAKE.classlibrary.Shots
{
    [ExcludeFromCodeCoverage]
    public class TriangleShot : Shot
    {
        public TriangleShot(Shot shot) : base(shot)
        {
            base.setShape("triangle");
        }
    }
}
