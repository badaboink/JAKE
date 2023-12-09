using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JAKE.classlibrary.Patterns.State;

namespace JAKE.classlibrary.Patterns.Memento
{
    public interface IMemento
    {
        IState GetState();

    }
}
