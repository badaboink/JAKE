using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace JAKE.classlibrary.Patterns
{
    public class XMLAdaptee
    {
        public string ConvertFromXML(string xmlString)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlString);

            // Extract values from XML
            List<string> values = new List<string>();
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
