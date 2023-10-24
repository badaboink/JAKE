using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JAKE.classlibrary.Patterns;

namespace JAKE.classlibrary
{
    public class RoundShot : Shot
    {
       public RoundShot(Shot shot) : base(shot)
        {
            base.setShape("round");
        }
    }
}
