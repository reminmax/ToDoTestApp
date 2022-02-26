using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FreshMvvm;
using PropertyChanged;
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
        private int _id;
        private IRestService _restService { get; }

        public TaskPageModel(IRestService restService)
        {
            _restService = restService;

            CreateTaskCommand = new FreshAwaitCommand(CreateTaskCommandHandler);

            AddValidationRules();
        }

        public ValidatableObject<string> UserName { get; set; }
        public ValidatableObject<string> Email { get; set; }
        public ValidatableObject<string> Text { get; set; }

        public FreshAwaitCommand CreateTaskCommand { get; }

        public LayoutState MainState { get; set; }

        public override void Init(object initData)
        {
            if (initData is null)
            {
                throw new ArgumentNullException(nameof(initData), "TaskPageModel.Init() parameter");
            }

            if (initData is not TaskModel taskModel)
            {
                throw new ArgumentException("Incorrect type parameter in TaskPageModel.Init()", nameof(initData));
            }

            UserName.Value = taskModel.UserName;
            Email.Value = taskModel.Email;
            Text.Value = taskModel.Text;
            _id = taskModel.Id;

            base.Init(initData);
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

        private async void CreateTaskCommandHandler(TaskCompletionSource<bool> obj)
        {
            await CreateTaskCommandHandler();
            obj.SetResult(true);
        }

        private async ValueTask CreateTaskCommandHandler()
        {
            if (!AreFieldsValid())
            {
                return;
            }

            MainState = LayoutState.Loading;

            try
            {
                bool isNewTask = _id ==0;


            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                MainState = LayoutState.None;
            }
        }

    }
}
