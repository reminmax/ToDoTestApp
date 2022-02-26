using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FreshMvvm;
using PropertyChanged;
using ToDoListApp.Helpers;
using ToDoListApp.Helpers.Validations;
using ToDoListApp.Helpers.Validations.Rules;
using ToDoListApp.Repository;
using Xamarin.CommunityToolkit.UI.Views;

namespace ToDoListApp.PageModels
{
    [AddINotifyPropertyChangedInterface]
    public class LoginPageModel : FreshBasePageModel
    {
        private IRestService _restService { get; }

        public LoginPageModel(IRestService restService)
        {
            _restService = restService;

            LoginCommand = new FreshAwaitCommand(LoginCommandHandler);

            AddValidationRules();

            MainState = LayoutState.None;
        }

        public LayoutState MainState { get; set; }

        public ValidatableObject<string> UserName { get; set; }
        public ValidatableObject<string> Password { get; set; }

        public FreshAwaitCommand LoginCommand { get; }

        private void AddValidationRules()
        {
            UserName = new ValidatableObject<string>();
            Password = new ValidatableObject<string>();

            UserName.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "Name is required." });
            Password.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "Password is required." });
        }

        private bool AreFieldsValid()
        {
            bool isUserNameValid = UserName.Validate();
            bool isPasswordValid = Password.Validate();

            return isUserNameValid && isPasswordValid;
        }

        private async void LoginCommandHandler(TaskCompletionSource<bool> obj)
        {
            await LoginAsync();
            obj.SetResult(true);
        }

        private async Task LoginAsync()
        {
            if (!AreFieldsValid())
                return;

            MainState = LayoutState.Loading;
            string methodName = "LoginPageModel.UpdateTaskListAsync()";

            try
            {
                var result = await _restService.LoginAsync(UserName.Value, Password.Value);
                if (result is null)
                {
                    throw new ArgumentNullException(nameof(result), methodName);
                }

                if (result.Message is null)
                {
                    throw new ArgumentNullException(nameof(result.Message), methodName);
                }

                if (result.Message is Dictionary<string, string> message)
                {
                    string token = string.Empty;
                    if (message.TryGetValue("token", out token))
                    {
                        // Save token
                        AppSettings.AuthToken = token;

                        ClearAuthData();

                        await CoreMethods.PushPageModel<MainPageModel>();
                    }
                    else
                    {
                        throw new ArgumentException("Token not received");
                    }
                }
                else
                {
                    throw new ArgumentException(methodName, nameof(message));
                }

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

        private void ClearAuthData()
        {
            UserName.Value = Password.Value = string.Empty;
        }
    }
}
