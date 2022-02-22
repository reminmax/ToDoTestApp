using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FreshMvvm;
using PropertyChanged;
using ToDoListApp.Helpers.Validations;
using ToDoListApp.Helpers.Validations.Rules;

namespace ToDoListApp.PageModels
{
    [AddINotifyPropertyChangedInterface]
    class LoginPageModel : FreshBasePageModel
    {
        public LoginPageModel()
        {
            LoginCommand = new FreshAwaitCommand(LoginAsync);

            AddValidationRules();
        }

        public ValidatableObject<string> Email { get; set; }
        public ValidatableObject<string> Password { get; set; }

        public FreshAwaitCommand LoginCommand { get; }

        protected override void ViewIsAppearing(object sender, EventArgs e)
        {
        }

        private void AddValidationRules()
        {
            Email = new ValidatableObject<string>();
            Password = new ValidatableObject<string>();

            Email.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "A email is required." });
            Email.Validations.Add(new IsValidEmailRule<string> { ValidationMessage = "Incorrect email" });
            Password.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "A password is required." });
        }

        private bool AreFieldsValid()
        {
            bool isEmailValid = Email.Validate();
            bool isPasswordValid = Password.Validate();

            return isEmailValid && isPasswordValid;
        }

        private async void LoginAsync(TaskCompletionSource<bool> obj)
        {
            await LoginCommandHandler();
            obj.SetResult(true);
        }

        private async ValueTask LoginCommandHandler()
        {
            if (AreFieldsValid())
            {}
        }

    }
}
