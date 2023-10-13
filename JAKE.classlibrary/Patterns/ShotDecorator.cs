using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary.Patterns
{
    public abstract class ShotDecorator : Shot
    {
        protected Shot _shot;

        protected ShotDecorator(Shot shot) : base()
        {
            _shot = shot;
        }
    }



}
