using JAKE.classlibrary.Patterns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace JAKE.classlibrary
{
    public class Controller
    {
        Command? slot;
        LinkedList<Command> commands;

        public Controller()
        {
            commands = new LinkedList<Command>();
        }

        public void SetCommand(Command command)
        {
            slot = command;
        }

        public void Execute()
        {
            if (slot == null)
            {
                return;
            }
            if (slot.Execute())
            {
                commands.AddFirst(slot);
            }
        }

        public void Undo()
        {
            if (commands.First != null)
            {
                commands.First.Value.Undo();
                commands.RemoveFirst();
            }
        }
    }
}
