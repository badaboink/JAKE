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
        Command slot;

        public Controller()
        {

        }

        public void SetCommand(Command command)
        {
            slot = command;
        }

        public void Execute()
        {
            slot.Execute();
        }
    }
}
