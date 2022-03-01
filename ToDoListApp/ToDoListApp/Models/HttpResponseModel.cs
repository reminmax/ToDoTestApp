using System;
using System.Collections.Generic;
using System.Linq;

namespace ToDoListApp.Models
{
    public class HttpResponseModel
    {
        public dynamic Message { get; set; }

        public string Status { get; set; }

        public int TotalTaskCount { get; set; }

        public static string GetMessageAsString(dynamic message)
        {
            if (message is string stringMessage) return stringMessage;

            if (message is Dictionary<string, string> dictionary)
                return string.Join(Environment.NewLine, dictionary.Values.Select(p => p));

            return string.Empty;
        }
    }
}
