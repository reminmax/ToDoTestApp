using FreshMvvm;
using Newtonsoft.Json;
using PropertyChanged;

namespace ToDoListApp.Models
{
    [AddINotifyPropertyChangedInterface]
    public class TaskModel : FreshBasePageModel
    {
        private string _imagePath;

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("username")]
        public string UserName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

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
        public int Status { get; set; }

        public string StatusAsString
        {
            get
            {
                return Status switch
                {
                    0 => "(0) Not completed",
                    1 => "(1) Not completed, changed by admin",
                    10 => "(10) Completed",
                    11 => "(11) Completed, changed by admin",
                    _ => "Unknown status"
                };
            }
        }
    }
}
