using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JAKE.classlibrary.Patterns;

namespace JAKE.classlibrary.Shots
{
    [ExcludeFromCodeCoverage]
    public class RoundShot : Shot
    {
       public RoundShot(Shot shot) : base(shot)
        {
            base.setShape("round");
        }
    }
}
