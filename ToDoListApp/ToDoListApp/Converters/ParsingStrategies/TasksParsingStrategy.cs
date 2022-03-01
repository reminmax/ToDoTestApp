using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ToDoListApp.Models;

namespace ToDoListApp.Converters.ParsingStrategies
{
    public class TasksParsingStrategy : IParsingStrategy
    {
        public object Parse(JToken request, JsonSerializer serializer)
        {
            return request["tasks"]?.ToObject<List<TaskModel>>(serializer);
        }

        public bool CanParse(JToken request)
        {
            return request.HasValues && request["tasks"] != null;
        }
    }
}
