using JAKE.classlibrary.Enemies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary.Patterns
{
    public interface IPrototype
    {
        public Enemy DeepClone();
        public Enemy ShallowClone();
    }
}
