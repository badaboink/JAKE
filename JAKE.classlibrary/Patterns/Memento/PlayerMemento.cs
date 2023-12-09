using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JAKE.classlibrary.Patterns.State;

namespace JAKE.classlibrary.Patterns.Memento
{
    public class PlayerMemento : IMemento
    {
        private IState state;

        public PlayerMemento(IState state)
        {
            this.state = state;
        }

        public IState GetState()
        {
            return state;
        }

    }
}
