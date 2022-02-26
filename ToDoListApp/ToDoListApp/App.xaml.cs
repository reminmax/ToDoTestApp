using System;
using FreshMvvm;
using ToDoListApp.Helpers;
using ToDoListApp.PageModels;
using ToDoListApp.Repository;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: ExportFont("FontAwesomeRegular.ttf", Alias = "FontAwesomeRegular")]

namespace ToDoListApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Register services for dependency injection
            FreshIOC.Container.Register<IRestService, RestService>().AsSingleton();

            // Set main page
            if (AppSettings.IsUserLoggedIn())
            {
                MainPage = new FreshNavigationContainer(
                    FreshPageModelResolver.ResolvePageModel<MainPageModel>());
            }
            else
            {
                MainPage = new FreshNavigationContainer(
                    //FreshPageModelResolver.ResolvePageModel<LoginPageModel>());
                    FreshPageModelResolver.ResolvePageModel<MainPageModel>());
            }
        }

        // TODO
        private void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("!!!" + e.ToString());
        }

        protected override void OnStart()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
