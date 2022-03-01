namespace ToDoListApp.Models
{
    public sealed class PaginationListItem
    {
        public PaginationListItem(int pageNumber, bool isSelected)
        {
            PageNumber = pageNumber;
            IsSelected = isSelected;
        }

        public int PageNumber { get; set; }
        public bool IsSelected { get; set; }
    }
}
