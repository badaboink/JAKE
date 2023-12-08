using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary.Patterns
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

        public void SetState(IState state)
        {
            this.state = state;
        }
    }
}
