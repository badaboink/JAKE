using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace JAKE.classlibrary.Patterns
{
    public class JsonAdapter : IStringAdapter
    {

        public string Convert(string jsonString)
        {
            JObject json = JObject.Parse(jsonString);

            // Extract values from JSON object
            List<string> values = new List<string>();
            foreach (var property in json.Properties())
            {
                values.Add(property.Value.ToString());
            }

            // Convert values to specific format
            return string.Join(":", values);
        }

       
    }

    /*
      static void Main(string[] args)
    {
        string xmlInput = "<data><name>agne</name><age>10</age><city>example city</city></data>";
        string consoleInput = "name:agne, age:10, city:example city";

        IStringAdapter xmlAdapter = new XMLAdapter();
        IStringAdapter consoleAdapter = new ConsoleAdapter();

        string resultXml = xmlAdapter.Convert(xmlInput);
        string resultConsole = consoleAdapter.Convert(consoleInput);

        Console.WriteLine("XML Result: " + resultXml);   // Output: agne:10:example city
        Console.WriteLine("Console Result: " + resultConsole);   // Output: agne:10:example city
    }
     */
}
