using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using FreshMvvm;
using PropertyChanged;
using ToDoListApp.Helpers;
using ToDoListApp.Models;
using ToDoListApp.Repository;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.CommunityToolkit.UI.Views;

namespace ToDoListApp.PageModels
{
    [AddINotifyPropertyChangedInterface]
    public class MainPageModel : FreshBasePageModel
    {
        private IRestService _restService { get; }
        private string _selectedSortField { get; set; }
        private int _paginationListSelectedNumber = 1;
        private int _totalTaskCount;

        public MainPageModel(IRestService restService)
        {
            _restService = restService;

            TaskList = new ObservableCollection<TaskModel>();
            PaginationList = new ObservableCollection<PaginationListItem>();

            AddNewTaskCommand = CommandFactory.Create(AddNewTaskAsync);
            NavigateToLoginPageCommand = CommandFactory.Create(NavigateToLoginPageAsync);
            ChangeSortDirectionCommand = CommandFactory.Create(ChangeSortDirectionAsync);
            ChangeSortFieldCommand = CommandFactory.Create<SortFieldModel>(ChangeSortFieldCommandAsync);
            PaginationListSelectionChangedCommand = CommandFactory.Create<PaginationListItem>(ExecutePaginationListSelectionAsync);
            TaskListSelectionChangedCommand = CommandFactory.Create<TaskModel>(OpenSelectedTaskAsync);

            FillSortFieldList();

            MainState = LayoutState.None;
        }

        public ObservableCollection<TaskModel> TaskList { get; private set; }

        public ObservableCollection<PaginationListItem> PaginationList { get; private set; }

        public SortFieldModel SelectedSortField { get; set; }

        public List<SortFieldModel> SortFieldList { get; private set; }

        public bool IsPaginationListVisible { get; private set; }

        public bool IsSortAscending { get; set; } = true;

        public LayoutState MainState { get; private set; }

        public IAsyncCommand<SortFieldModel> ChangeSortFieldCommand { get; }
        public IAsyncCommand ChangeSortDirectionCommand { get; }
        public IAsyncCommand AddNewTaskCommand { get; }
        public IAsyncCommand NavigateToLoginPageCommand { get; }
        public IAsyncCommand<PaginationListItem> PaginationListSelectionChangedCommand { get; }

        public IAsyncCommand<TaskModel> TaskListSelectionChangedCommand { get; }

        private void TaskListCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdatePaginationList();
        }

        private void UpdatePaginationList()
        {
            PaginationList.Clear();

            if (!TaskList.Any()) return;

            int taskItemsNumberPerPage = ConstantValues.TaskItemsNumberPerPage;
            double itemsCount = (double)_totalTaskCount / taskItemsNumberPerPage;
            int roundedItemsCount = (int)Math.Ceiling(itemsCount);

            for (int i = 0; i < roundedItemsCount; i++)
            {
                PaginationList.Add(
                    new PaginationListItem(i + 1, _paginationListSelectedNumber == i + 1));
            }
        }

        private void FillSortFieldList()
        {
            SortFieldList = new List<SortFieldModel>()
            {
                new SortFieldModel("id", "Id"),
                new SortFieldModel("username", "User name"),
                new SortFieldModel("email", "Email"),
                new SortFieldModel("status", "Status"),
            };

            SelectedSortField = SortFieldList.First();
        }

        private async Task NavigateToLoginPageAsync()
        {
            await CoreMethods.PushPageModel<LoginPageModel>();
        }

        private async Task AddNewTaskAsync()
        {
            await CoreMethods.PushPageModel<TaskPageModel>(new TaskModel());
        }

        private async Task OpenSelectedTaskAsync(TaskModel taskModel)
        {
            if (!AppSettings.IsUserLoggedIn())
            {
                await CoreMethods.DisplayAlert("Access denied",
                    "Editing is allowed only to authorized user.",
                    "OK");
                return;
            }

            await CoreMethods.PushPageModel<TaskPageModel>(taskModel);
        }

        private async Task ExecutePaginationListSelectionAsync(PaginationListItem item)
        {
            if (item is null) return;

            _paginationListSelectedNumber = item.PageNumber;

            await UpdateTaskListAsync();
        }

        private async Task ChangeSortFieldCommandAsync(SortFieldModel item)
        {
            if (item is null) return;

            _selectedSortField = item.Id;
            await UpdateTaskListAsync();
        }

        private async Task ChangeSortDirectionAsync()
        {
            IsSortAscending = !IsSortAscending;

            await UpdateTaskListAsync();
        }

        protected override async void ViewIsAppearing(object sender, EventArgs e)
        {
            base.ViewIsAppearing(sender, e);

            await UpdateTaskListAsync();
        }

        private async Task UpdateTaskListAsync()
        {
            MainState = LayoutState.Loading;

            TaskList.Clear();

            try
            {
                var result = await _restService.GetTaskListAsync(sortField: _selectedSortField,
                    sortDirection: IsSortAscending ? "asc" : "desc",
                    pageNumber: _paginationListSelectedNumber);

                if (result is null)
                    throw new ArgumentNullException(nameof(result));

                if (result.Message is null)
                    throw new ArgumentNullException(nameof(result.Message));

                var message = result.Message as List<TaskModel>;
                if (message is null)
                    throw new ArgumentException(nameof(message));

                foreach (var item in message)
                {
                    TaskList.Add(item);
                }

                _totalTaskCount = result.TotalTaskCount;
                IsPaginationListVisible = _totalTaskCount > 0;
            }
            catch (Exception ex)
            {
                await CoreMethods.DisplayAlert("Error", ex.Message, "Ok");
            }
            finally
            {
                MainState = LayoutState.None;

                UpdatePaginationList();
            }
        }
    }
}
