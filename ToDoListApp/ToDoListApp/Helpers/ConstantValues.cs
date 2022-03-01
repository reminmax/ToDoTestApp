namespace ToDoListApp.Helpers
{
    public static class ConstantValues
    {
        public static readonly int TokenExpirationTimeHours = 24;

        public static readonly int TaskItemsNumberPerPage = 3;

        public static readonly string SuccessStatusString = "ok";

        public static string BaseUri = "https://uxcandy.com/~shapoval/test-task-backend/v2";
        public static readonly string LoginUri = "/login";
        public static readonly string TaskListUri = "/";
        public static readonly string AddNewTaskUri = "/create";
        public static readonly string EditTaskUri = "/edit/";
        public static readonly string DeveloperName = "Remin";
    }
}
