using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary.Patterns.Interpreter
{
    public class SpecialSymbolInterpreter : IMessageInterpreter
    {
        public string interpret(string message)
        {
            return message.Replace("#heart", " ♥ ").Replace("#star", " ★ ").Replace("#happy", " ☺ ")
                .Replace("#sad", " ☹ ").Replace("#good", " 👍 ").Replace("#bad", " 👎 ");
        }
    }
}
