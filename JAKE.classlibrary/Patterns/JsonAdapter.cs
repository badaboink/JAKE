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

        private JsonAdaptee _jsonConverter = new JsonAdaptee();

        public string Convert(string inputString)
        {
            return _jsonConverter.ConvertFromJson(inputString);
        }
    }

       
}
