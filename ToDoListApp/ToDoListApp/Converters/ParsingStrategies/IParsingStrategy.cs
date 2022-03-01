using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ToDoListApp.Converters.ParsingStrategies
{
    public interface IParsingStrategy
    {
        object Parse(JToken request, JsonSerializer serializer);

        bool CanParse(JToken request);
    }
}
