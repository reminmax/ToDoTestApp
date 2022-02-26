using System;
using Newtonsoft.Json;
using ToDoListApp.Models;

namespace ToDoListApp.Converters
{
    class TaskStatusJsonConverter : JsonConverter<TaskStatuses>
    {
        public override TaskStatuses ReadJson(JsonReader reader, Type objectType, TaskStatuses existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.Value is null)
                throw new ArgumentNullException(nameof(reader.Value));

            try
            {
                return (TaskStatuses)Enum.Parse(typeof(TaskStatuses), reader.Value.ToString());
            }
            catch
            {
                throw new ArgumentException(string.Join(" ", "TaskStatusJsonConverter", nameof(reader.Value)));
            }
        }

        public override void WriteJson(JsonWriter writer, TaskStatuses value, JsonSerializer serializer)
        {
            writer.WriteValue((byte)value);
        }
    }
}
