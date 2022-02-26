using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ToDoListApp.Pages.Fragments
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoadingView : StackLayout
    {
        public LoadingView()
        {
            InitializeComponent();
        }
    }
}