using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary
{
    public class JsonAdaptee
    {
        public string ConvertFromJson(string jsonString)
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
}
