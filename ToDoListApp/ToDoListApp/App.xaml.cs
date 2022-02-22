using System;
using FreshMvvm;
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
            MainPage = new FreshNavigationContainer(
                FreshPageModelResolver.ResolvePageModel<MainPageModel>());
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
