using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace JAKE.classlibrary.Patterns
{
    public class XmlAdaptee
    {
#pragma warning disable CA1822 // Mark members as static
        public string ConvertFromXML(string xmlString)
#pragma warning restore CA1822 // Mark members as static
        {
            XmlDocument xmlDoc = new();
            xmlDoc.LoadXml(xmlString);

            // Extract values from XML
            List<string> values = new();
            if(xmlDoc.DocumentElement == null)
            {
                return "";
            }
            foreach (XmlNode node in xmlDoc.DocumentElement.ChildNodes)
            {
                values.Add(node.InnerText);
            }

            // Convert values to specific format
            return string.Join(":", values);
        }
    }
}
