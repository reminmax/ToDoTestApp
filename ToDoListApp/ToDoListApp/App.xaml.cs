using System;
using FreshMvvm;
using ToDoListApp.Helpers;
using ToDoListApp.PageModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ToDoListApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Set main page
            if (AppSettings.IsUserLoggedIn())
            {
                MainPage = new FreshNavigationContainer(
                    FreshPageModelResolver.ResolvePageModel<MainPageModel>());
            }
            else
            {
                MainPage = new FreshNavigationContainer(
                    FreshPageModelResolver.ResolvePageModel<LoginPageModel>());
            }
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
