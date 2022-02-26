using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using FreshMvvm;
using PropertyChanged;
using ToDoListApp.Models;
using ToDoListApp.Repository;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;

namespace ToDoListApp.PageModels
{
    [AddINotifyPropertyChangedInterface]
    public class MainPageModel : FreshBasePageModel
    {
        private IRestService _restService { get; }
        private TaskModel _taskListSelectedItem;
        private string _selectedSortField { get; set; }

        public MainPageModel(IRestService restService)
        {
            _restService = restService;

            TaskList = new ObservableCollection<TaskModel>();
            TaskList.CollectionChanged += TaskList_CollectionChanged;

            PaginationList = new ObservableCollection<PaginationListItem>();

            ChangeSortDirectionCommand = CommandFactory.Create(ChangeSortDirectionAsync);
            ChangeSortFieldCommand = CommandFactory.Create<SoftFieldModel>(ChangeSortFieldCommandAsync);

            FillSortFieldList();

            MainState = LayoutState.None;
        }

        public ObservableCollection<TaskModel> TaskList { get; private set; }

        public ObservableCollection<PaginationListItem> PaginationList { get; private set; }

        public List<SoftFieldModel> SortFieldList { get; private set; }

        public bool IsSortAscending { get; set; } = true;

        public LayoutState MainState { get; private set; }

        public IAsyncCommand<SoftFieldModel> ChangeSortFieldCommand { get; }
        public IAsyncCommand ChangeSortDirectionCommand { get; }

        public TaskModel TaskListSelectedItem
        {
            get => _taskListSelectedItem;
            set
            {
                _taskListSelectedItem = value;
                RaisePropertyChanged();

                NavigateToTaskPageAsync(TaskListSelectedItem);
            }
        }

        private void TaskList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //if (!TaskList.Any())
            //{
            //    CategoriesList.Clear();
            //    DisplayedTaskList.Clear();
            //    return;
            //}

            //UpdateCategoriesList();

            //// Update the displayed list based on the selected category
            //UpdateDisplayedTaskList();
        }

        private void FillSortFieldList()
        {
            SortFieldList = new List<SoftFieldModel>()
            {
                new SoftFieldModel("id", "Id"),
                new SoftFieldModel("username", "User name"),
                new SoftFieldModel("email", "Email"),
                new SoftFieldModel("status", "Status"),
            };

            //SelectedSortField = SortFieldList[0];
        }

        private async Task ChangeSortFieldCommandAsync(SoftFieldModel item)
        {
            if (item is SoftFieldModel selectedItem)
            {
                _selectedSortField = selectedItem.Id;
                await UpdateTaskListAsync();
            }
        }

        private async Task ChangeSortDirectionAsync()
        {
            IsSortAscending = !IsSortAscending;

            await UpdateTaskListAsync();
        }

        private async void NavigateToTaskPageAsync(TaskModel taskModel)
        {
            await CoreMethods.PushPageModel<TaskPageModel>(taskModel);
        }

        protected override async void ViewIsAppearing(object sender, EventArgs e)
        {
            await UpdateTaskListAsync();

            //UpdateTaskListAsync().ContinueWith((tResult) =>
            //{
            //    throw new Exception();
            //}, TaskContinuationOptions.OnlyOnFaulted);

            base.ViewIsAppearing(sender, e);
        }

        private async Task UpdateTaskListAsync()
        {
            MainState = LayoutState.Loading;
            TaskList.Clear();
            string methodName = "MainPageModel.UpdateTaskListAsync()";

            try
            {
                var result = await _restService.GetTaskListAsync(sortField: _selectedSortField,
                    sortDirection: IsSortAscending? "asc" : "desc");
                if (result is null)
                {
                    throw new ArgumentNullException(nameof(result), methodName);
                }

                if (result.Message is null)
                {
                    throw new ArgumentNullException(nameof(result.Message), methodName);
                }

                var message = result.Message as List<TaskModel>;
                if (message is null)
                {
                    throw new ArgumentException(methodName, nameof(message));
                }

                foreach (var item in message)
                {
                    TaskList.Add(item);
                }
            }
            catch (Exception ex)
            {
                await CoreMethods.DisplayAlert(methodName, ex.Message, "Ok");
            }
            finally
            {
                MainState = LayoutState.None;
            }
        }
    }
}
