using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace JAKE.classlibrary.Patterns
{
    public class XMLAdapter : IStringAdapter
    {
        private XMLAdaptee _xmlConverter = new XMLAdaptee();

        public string Convert(string inputString)
        {
            return _xmlConverter.ConvertFromXML(inputString);
        }

        public int CountCharacters(string inputString)
        {
            return inputString.Length;
        }
    }
}
