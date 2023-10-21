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
        public string Convert(string xmlString)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlString);

            // Extract values from XML
            List<string> values = new List<string>();
            foreach (XmlNode node in xmlDoc.DocumentElement.ChildNodes)
            {
                values.Add(node.InnerText);
            }

            // Convert values to specific format
            return string.Join(":", values);
        }
    }
}
