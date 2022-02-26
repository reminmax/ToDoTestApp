using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using ToDoListApp.Converters;

namespace ToDoListApp.Models
{
    [JsonConverter(typeof(HttpResponseJsonConverter))]
    public class HttpResponseModel
    {
        public dynamic Message { get; set; }

        public string Status { get; set; }

        public int TotalTaskCount { get; set; }
    }
}
