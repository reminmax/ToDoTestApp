using System;
using System.Threading.Tasks;
using System.Windows.Input;
using FreshMvvm;
using PropertyChanged;
using ToDoListApp.Helpers;
using ToDoListApp.Helpers.Validations;
using ToDoListApp.Helpers.Validations.Rules;
using ToDoListApp.Models;
using ToDoListApp.Repository;
using Xamarin.CommunityToolkit.UI.Views;

namespace ToDoListApp.PageModels
{
    [AddINotifyPropertyChangedInterface]
    public class TaskPageModel : FreshBasePageModel
    {
        private TaskModel _task;

        public TaskPageModel(IRestService restService)
        {
            _restService = restService;

            _task = new TaskModel();

            AddValidationRules();
        }

        private IRestService _restService { get; }

        public ValidatableObject<string> UserName { get; set; }
        public ValidatableObject<string> Email { get; set; }
        public ValidatableObject<string> Text { get; set; }

        public string Title { get; private set; }

        public LayoutState MainState { get; set; }

        public bool IsNewTask => _task.Id == 0;
        public bool IsTaskCompleted { get; set; }

        public ICommand CreateTaskCommand
        {
            get
            {
                return new FreshAwaitCommand(async (tcs) =>
                {
                    await CreateOrUpdateTaskAsync();
                    tcs.SetResult(true);
                });
            }
        }

        public override void Init(object initData)
        {
            base.Init(initData);

            if (initData is TaskModel taskModel)
            {
                _task = taskModel;

                UserName.Value = taskModel.UserName;
                Email.Value = taskModel.Email;
                Text.Value = taskModel.Text;
                _task.Status = taskModel.Status;

                IsTaskCompleted = taskModel.Status >= 10;
                Title = _task.Id == 0 ? "Add new task" : "Edit task";
            }
        }

        private void AddValidationRules()
        {
            UserName = new ValidatableObject<string>();
            Email = new ValidatableObject<string>();
            Text = new ValidatableObject<string>();

            UserName.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "User name is required." });
            Email.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "Email is required." });
            Email.Validations.Add(new IsValidEmailRule<string> { ValidationMessage = "Incorrect email format." });
            Text.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "Text is required." });
        }

        private bool AreFieldsValid()
        {
            bool isUserNameValid = UserName.Validate();
            bool isEmailValid = Email.Validate();
            bool isTextValid = Text.Validate();

            return isUserNameValid && isEmailValid && isTextValid;
        }

        private async ValueTask CreateOrUpdateTaskAsync()
        {
            if (!AreFieldsValid()) return;

            if (IsNewTask)
                await CreateTaskAsync();
            else
                await UpdateTaskAsync();
        }

        private async ValueTask CreateTaskAsync()
        {
            MainState = LayoutState.Loading;

            try
            {
                HttpResponseModel result = await _restService.AddNewTaskAsync(UserName.Value, Email.Value, Text.Value);

                if (result is null)
                    throw new ArgumentNullException(nameof(result));

                if (result.Status != ConstantValues.SuccessStatusString)
                {
                    await CoreMethods.DisplayAlert("Error", HttpResponseModel.GetMessageAsString(result.Message), "OK");
                    return;
                }

                await CoreMethods.DisplayAlert("Info", "New task was successfully saved!", "Ok");
                await CoreMethods.PopPageModel();
            }
            catch (Exception ex)
            {
                await CoreMethods.DisplayAlert("Error", ex.Message, "Ok");
            }
            finally
            {
                MainState = LayoutState.None;
            }
        }

        private int GetNewTaskStatus()
        {
            bool textHasChanged = _task.Text != Text.Value;
            bool statusHasChanged = IsTaskCompleted
                ? (_task.Status == 0 || _task.Status == 1)
                : (_task.Status == 10 || _task.Status == 11);

            bool somethingHasChanged = textHasChanged || statusHasChanged;

            return !IsTaskCompleted
                ? 0 + (somethingHasChanged ? 1 : 0)
                : 10 + (somethingHasChanged ? 1 : 0);
        }

        private async ValueTask UpdateTaskAsync()
        {
            MainState = LayoutState.Loading;

            int newStatus = GetNewTaskStatus();

            try
            {
                HttpResponseModel result = await _restService.EditTaskAsync(_task.Id, Text.Value, newStatus);

                if (result is null)
                    throw new ArgumentNullException(nameof(result));

                if (result.Status != ConstantValues.SuccessStatusString)
                {
                    await CoreMethods.DisplayAlert("Error", HttpResponseModel.GetMessageAsString(result.Message), "OK");
                    return;
                }

                await CoreMethods.PopPageModel();
            }
            catch (Exception ex)
            {
                await CoreMethods.DisplayAlert("Error", ex.Message, "Ok");
            }
            finally
            {
                MainState = LayoutState.None;
            }
        }
    }
}
