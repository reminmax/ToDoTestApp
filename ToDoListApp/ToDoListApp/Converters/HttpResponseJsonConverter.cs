#nullable enable
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using ToDoListApp.Models;

namespace ToDoListApp.Converters
{
    public class HttpResponseJsonConverter : JsonConverter
    {
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
            // Message field value type determined by json content
            var messageToken = token["message"];
            if (messageToken is not null)
            {
                if (!messageToken.HasValues)
                {
                    obj.Message = messageToken.Value<string>();
                }
                else
                {
                    if (messageToken["tasks"] is not null)
                    {
                        obj.Message = messageToken["tasks"]?.ToObject<List<TaskModel>>(serializer);
                    }
                    else
                    {
                        obj.Message = messageToken.ToObject<Dictionary<string, string>>();
                    }

                    // total_task_count
                    if (messageToken["total_task_count"] != null)
                    {
                        obj.TotalTaskCount = messageToken["total_task_count"].Value<int>();
                    }
                }
            }

            // status
            obj.Status = token["status"]?.Value<string>();

            return obj;
        }

        public override bool CanConvert(Type objectType) => objectType == typeof(HttpResponseModel);
    }
}
