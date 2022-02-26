using System;
using System.Collections.Generic;
using System.Text;

namespace ToDoListApp.Models
{
    public class SoftFieldModel
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public SoftFieldModel(string id, string name)
        {
            Id = id;
            Name = name;
        }
        public override string ToString()
        {
            return Name;
        }
    }
}
