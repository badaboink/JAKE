using JAKE.classlibrary.Patterns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary
{
    public class ServerString
    {
        public string ConvertedString { get; }

        private readonly IStringAdapter _adapter;

        public ServerString(string inputString)
        {
            if (inputString.Contains('{') && inputString.Contains('}'))
            {
                _adapter = new JsonAdapter();
            }
            else if (inputString.StartsWith('<') && inputString.EndsWith('>'))
            {
                _adapter = new XmlAdapter();
            }
            else if (inputString.Contains(':') && inputString.Contains(','))
            {
                _adapter = new ConsoleAdapter();
            }
            else
            {
                throw new ArgumentException("Unsupported input format");
            }

            ConvertedString = _adapter.Convert(inputString);
        }

       
    }
}
