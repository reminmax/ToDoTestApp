using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ToDoListApp.Converters.ParsingStrategies
{
    public class DictionaryParsingStrategy : IParsingStrategy
    {
        public object Parse(JToken request, JsonSerializer serializer)
        {
            return request?.ToObject<Dictionary<string, string>>();
        }

        public bool CanParse(JToken request)
        {
            return request.HasValues && request["tasks"] == null;
        }
    }
}
