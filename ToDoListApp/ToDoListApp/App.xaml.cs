using System;
using FreshMvvm;
using ToDoListApp.PageModels;
using ToDoListApp.Repository;
using Xamarin.Forms;

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
