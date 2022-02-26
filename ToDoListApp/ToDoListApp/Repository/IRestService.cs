using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ToDoListApp.Models;

namespace ToDoListApp.Repository
{
    public interface IRestService
    {
        Task<HttpResponseModel> LoginAsync(string userName, string password);

        Task<HttpResponseModel> GetTaskListAsync(string sortField = "id", string sortDirection = "asc", int pageNumber = 1);
    }
}
