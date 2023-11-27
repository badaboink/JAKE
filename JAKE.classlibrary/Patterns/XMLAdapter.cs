using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace JAKE.classlibrary.Patterns
{
    public class XmlAdapter : IStringAdapter
    {
        private readonly XmlAdaptee _xmlConverter = new XmlAdaptee();

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
