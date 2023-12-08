using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary.Patterns
{
    public class Caretaker
    {
        private List<IMemento> mementos = new List<IMemento>();

        public void AddMemento(IMemento memento)
        {
            mementos.Add(memento);
        }

        public IMemento GetMemento(int index)
        {
            if (index >= 0 && index < mementos.Count)
            {
                return mementos[index];
            }

            return null;
        }

        public int GetMementosCount()
        {
            return mementos.Count;
        }
    }
}
