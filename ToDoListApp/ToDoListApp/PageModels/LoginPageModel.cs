using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using FreshMvvm;
using PropertyChanged;
using ToDoListApp.Helpers;
using ToDoListApp.Helpers.Validations;
using ToDoListApp.Helpers.Validations.Rules;
using ToDoListApp.Models;
using ToDoListApp.Repository;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;

namespace ToDoListApp.PageModels
{
    [AddINotifyPropertyChangedInterface]
    public class LoginPageModel : FreshBasePageModel
    {
        public LoginPageModel(IRestService restService)
        {
            _restService = restService;

            LoginCommand = new FreshAwaitCommand(LoginCommandHandler);
            LogoutCommand = new Command(ExecuteLogout);
            NavigateToMainPageCommand = CommandFactory.Create(NavigateToMainPageAsync);

            AddValidationRules();

            MainState = LayoutState.None;
            AuthorizedUserName = AppSettings.UserName;
        }

        private IRestService _restService { get; }

        public LayoutState MainState { get; set; }
        public string CustomState { get; set; }
        public string AuthorizedUserName { get; private set; }

        public ValidatableObject<string> UserName { get; set; }
        public ValidatableObject<string> Password { get; set; }

        public FreshAwaitCommand LoginCommand { get; }
        public ICommand LogoutCommand { get; }
        public IAsyncCommand NavigateToMainPageCommand { get; }

        protected override void ViewIsAppearing(object sender, EventArgs e)
        {
            base.ViewIsAppearing(sender, e);

            MainState = !AppSettings.IsUserLoggedIn() ? LayoutState.None : LayoutState.Success;
        }

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

        private async Task NavigateToMainPageAsync() => await CoreMethods.PushPageModel<MainPageModel>();

        private void ExecuteLogout()
        {
            AppSettings.UserName = string.Empty;
            AppSettings.AuthToken = string.Empty;

            MainState = LayoutState.None;
        }

        private async void LoginCommandHandler(TaskCompletionSource<bool> obj)
        {
            await LoginAsync();
            obj.SetResult(true);
        }

        private string TryGetToken(dynamic message)
        {
            if (message is Dictionary<string, string> dictionary)
            {
                string token = string.Empty;
                if (dictionary.TryGetValue("token", out token))
                {
                    return token;
                }
            }

            return string.Empty;
        }

        private async Task LoginAsync()
        {
            if (!AreFieldsValid()) return;

            MainState = LayoutState.Loading;

            try
            {
                var result = await _restService.LoginAsync(UserName.Value, Password.Value);
                if (result is null)
                    throw new ArgumentNullException(nameof(result));

                if (result.Message is null)
                    throw new ArgumentNullException(nameof(result.Message));

                if (result.Status != ConstantValues.SuccessStatusString)
                {
                    string errorDetails = HttpResponseModel.GetMessageAsString(result.Message);
                    await CoreMethods.DisplayAlert("Error", errorDetails, "OK");
                    return;
                }

                string token = TryGetToken(result.Message);
                if (string.IsNullOrEmpty(token))
                    throw new ArgumentException("Token not received");

                // Save token
                AppSettings.AuthToken = token;
                AppSettings.UserName = UserName.Value;

                ClearAuthData();

                await CoreMethods.PushPageModel<MainPageModel>();
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
