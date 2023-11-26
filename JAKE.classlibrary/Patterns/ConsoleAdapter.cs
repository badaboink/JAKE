using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary.Patterns
{
    public class ConsoleAdapter : IStringAdapter
    {
        private readonly ConsoleAdaptee _consoleConverter = new ConsoleAdaptee();

        public string Convert(string inputString)
        {
            return _consoleConverter.ConvertFromConsole(inputString);
        }
        public int CountCharacters(string inputString)
        {
            return inputString.Length;
        }
    }
}
