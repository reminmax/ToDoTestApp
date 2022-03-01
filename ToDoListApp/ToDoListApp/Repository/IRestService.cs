using System.Threading.Tasks;
using ToDoListApp.Models;

namespace ToDoListApp.Repository
{
    public interface IRestService
    {
        Task<HttpResponseModel> LoginAsync(string userName, string password);

        Task<HttpResponseModel> GetTaskListAsync(string sortField = "id", string sortDirection = "asc", int pageNumber = 1);

        Task<HttpResponseModel> AddNewTaskAsync(string userName, string email, string text);

        Task<HttpResponseModel> EditTaskAsync(int id, string text, int status);
    }
}
