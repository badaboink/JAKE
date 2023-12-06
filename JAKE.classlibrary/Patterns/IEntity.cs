using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary.Patterns
{
    public interface IEntity
    {
        void Accept(IGameEntityVisitor visitor);
    }
}
