using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JAKE.classlibrary.Patterns.Interpreter
{
    public class ProfanityFilterInterpreter : IMessageInterpreter
    {
        private readonly List<string> profanities = new List<string> { "fuck", "nx", "bitch", "krw", "kurva", 
            "kurwa", "zertva", "debile", "daunas", "die", "padla", "dalbajobe", "kekse" };

        public string interpret(string message)
        {
            foreach (var profanity in profanities)
            {
                string pattern = $@"{Regex.Escape(profanity)}";
                message = Regex.Replace(message, pattern, "###", RegexOptions.IgnoreCase);
            }
            return message;
        }
    }
}
