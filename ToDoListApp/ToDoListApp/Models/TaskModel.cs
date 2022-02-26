using System;
using FreshMvvm;
using Newtonsoft.Json;
using PropertyChanged;
using ToDoListApp.Converters;

namespace ToDoListApp.Models
{
    [AddINotifyPropertyChangedInterface]
    public class TaskModel : FreshBasePageModel, IEquatable<TaskModel>
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("username")]
        public string UserName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        private string _imagePath;

        [JsonProperty("image_path")]
        public string ImagePath
        {
            get => string.IsNullOrEmpty(_imagePath) ? "no_image.png" : _imagePath;
            set
            {
                _imagePath = value;
                RaisePropertyChanged();
            }
        }

        [JsonProperty("status")]
        [JsonConverter(typeof(TaskStatusJsonConverter))]
        public TaskStatuses Status { get; set; }

        public string StatusAsString
        {
            get
            {
                return Status switch
                {
                    TaskStatuses.Created => "(0) Created",
                    TaskStatuses.CreatedChangedByAdmin => "(1) Created and changed by admin",
                    TaskStatuses.Done => "(10) Done",
                    TaskStatuses.DoneChangedByAdmin => "(11) Done and changed by admin",
                    _ => "Unknown status"
                };
            }
        }

        [JsonIgnore]
        public bool IsDone
        {
            get => Status is TaskStatuses.Done or TaskStatuses.DoneChangedByAdmin;
            set
            {
                if (value)
                {
                    Status = TaskStatuses.Done;
                }
                else
                {
                    Status = TaskStatuses.DoneChangedByAdmin;
                }
            }
        }

        public bool Equals(TaskModel other)
        {
            TaskModel taskObj = other as TaskModel;
            if (taskObj == null)
                return false;

            return Id.Equals(taskObj.Id)
                   && UserName.Equals(taskObj.UserName)
                   && Email.Equals(taskObj.Email)
                   && Text.Equals(taskObj.Text)
                   && Status.Equals(taskObj.Status);
        }

        public static bool operator ==(TaskModel obj1, TaskModel obj2)
        {
            if (obj1 is null || obj2 is null)
                return false;

            return obj1.Id == obj2.Id
                   && obj1.UserName == obj2.UserName
                   && obj1.Email == obj2.Email
                   && obj1.Text == obj2.Text
                   && obj1.Status == obj2.Status;
        }

        public static bool operator !=(TaskModel obj1, TaskModel obj2) => !(obj1 == obj2);

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
