using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary.Patterns
{
    public class ConsoleAdapter : IStringAdapter
    {
        public string Convert(string consoleString)
        {
            // Extract values from console input
            List<string> values = new List<string>();
            string[] pairs = consoleString.Split(',');

            foreach (var pair in pairs)
            {
                string[] keyValue = pair.Trim().Split(':');
                if (keyValue.Length == 2)
                {
                    values.Add(keyValue[1].Trim());
                }
            }

            // Convert values to specific format
            return string.Join(":", values);
        }
    }
}
