#nullable enable
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ToDoListApp.Converters.ParsingStrategies;
using ToDoListApp.Models;

namespace ToDoListApp.Converters
{
    public class HttpResponseJsonConverter : JsonConverter
    {
        private static List<IParsingStrategy>? _parsingStrategies;

        public HttpResponseJsonConverter(List<IParsingStrategy> parsingStrategies)
        {
            if (parsingStrategies is null)
                throw new ArgumentNullException(nameof(parsingStrategies));

            _parsingStrategies = parsingStrategies;
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var token = JToken.Load(reader);
            if (token.Type is JTokenType.Null)
            {
                return null;
            }

            var obj = new HttpResponseModel();

            // message
            var messageToken = token["message"];
            if (messageToken != null)
            {
                foreach (var strategy in _parsingStrategies)
                {
                    if (strategy.CanParse(messageToken))
                    {
                        obj.Message = strategy.Parse(messageToken, serializer);
                        break;
                    }
                }

                // total_task_count
                if (messageToken["total_task_count"] != null)
                {
                    obj.TotalTaskCount = messageToken["total_task_count"].Value<int>();
                }
            }

            // status
            obj.Status = token["status"]?.Value<string>();

            return obj;
        }

        public override bool CanConvert(Type objectType) => objectType == typeof(HttpResponseModel);
    }
}
