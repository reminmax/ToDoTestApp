using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ToDoListApp.Pages.Fragments
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EmptyTaskListStateView : StackLayout
    {
        public EmptyTaskListStateView()
        {
            InitializeComponent();
        }
    }
}