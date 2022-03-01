using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ToDoListApp.Converters.ParsingStrategies
{
    public class StringParsingStrategy : IParsingStrategy
    {
        public object Parse(JToken request, JsonSerializer serializer)
        {
            return request?.Value<string>();
        }

        public bool CanParse(JToken request)
        {
            return !request.HasValues;
        }
    }
}
