using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ToDoListApp.Converters;
using ToDoListApp.Converters.ParsingStrategies;
using ToDoListApp.Helpers;
using ToDoListApp.Models;

namespace ToDoListApp.Repository
{
    public class RestService : IRestService
    {
        private readonly Uri _baseUri = new Uri(ConstantValues.BaseUri);
        private readonly string _developerName = ConstantValues.DeveloperName;
        private readonly List<IParsingStrategy> _parsingStrategies;
        private HttpClient _httpClient;

        public RestService()
        {
            HttpClientHandler clientHandler = new HttpClientHandler()
            {
                // added to avoid "The SSL connection could not be established" error
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };

            _httpClient = new HttpClient(clientHandler);

            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Parsing strategies
            _parsingStrategies = new List<IParsingStrategy>()
            {
                new TasksParsingStrategy(),
                new StringParsingStrategy(),
                new DictionaryParsingStrategy()
            };
        }

        public async Task<HttpResponseModel> GetTaskListAsync(string sortField, string sortDirection, int pageNumber)
        {
            var builder = new UriBuilderExt(string.Concat(_baseUri, ConstantValues.TaskListUri));
            builder.AddParameter("developer", _developerName);
            builder.AddParameter("sort_field", sortField);
            builder.AddParameter("sort_direction", sortDirection);
            builder.AddParameter("page", pageNumber.ToString());
            var uri = builder.Uri;

            try
            {
                using (var response = await _httpClient.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead))
                {
                    response.EnsureSuccessStatusCode();

                    string content = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<HttpResponseModel>(content, new HttpResponseJsonConverter(_parsingStrategies));

                    return result;
                }
            }
            catch (HttpRequestException)
            {
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<HttpResponseModel> LoginAsync(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException(nameof(userName));

            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException(nameof(password));

            var builder = new UriBuilderExt(string.Concat(_baseUri, ConstantValues.LoginUri));
            builder.AddParameter("developer", _developerName);
            var uri = builder.Uri;

            try
            {
                using (var formData = new MultipartFormDataContent())
                {
                    formData.Add(new StringContent(userName), "\"username\"");
                    formData.Add(new StringContent(password), "\"password\"");

                    var response = await _httpClient.PostAsync(uri, formData);

                    response.EnsureSuccessStatusCode();

                    string content = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<HttpResponseModel>(content, new HttpResponseJsonConverter(_parsingStrategies));

                    return result;
                }
            }
            catch (HttpRequestException)
            {
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<HttpResponseModel> AddNewTaskAsync(string userName, string email, string text)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException(nameof(userName));

            if (string.IsNullOrEmpty(email))
                throw new ArgumentNullException(nameof(email));

            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException(nameof(text));

            var builder = new UriBuilderExt(string.Concat(_baseUri, ConstantValues.AddNewTaskUri));
            builder.AddParameter("developer", _developerName);
            var uri = builder.Uri;

            try
            {
                using (var formData = new MultipartFormDataContent())
                {
                    formData.Add(new StringContent(userName), "\"username\"");
                    formData.Add(new StringContent(email), "\"email\"");
                    formData.Add(new StringContent(text), "\"text\"");

                    var response = await _httpClient.PostAsync(uri, formData);

                    response.EnsureSuccessStatusCode();

                    string content = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<HttpResponseModel>(content, new HttpResponseJsonConverter(_parsingStrategies));

                    return result;
                }
            }
            catch (HttpRequestException)
            {
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<HttpResponseModel> EditTaskAsync(int id, string text, int status)
        {
            if (id == 0)
                throw new ArgumentNullException(nameof(id));

            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException(nameof(text));

            var builder = new UriBuilderExt(string.Concat(_baseUri, ConstantValues.EditTaskUri, id.ToString()));
            builder.AddParameter("developer", _developerName);
            var uri = builder.Uri;

            try
            {
                using (var formData = new MultipartFormDataContent())
                {
                    formData.Add(new StringContent(AppSettings.AuthToken), "\"token\"");
                    formData.Add(new StringContent(text), "\"text\"");
                    formData.Add(new StringContent(status.ToString()), "\"status\"");

                    var response = await _httpClient.PostAsync(uri, formData);

                    response.EnsureSuccessStatusCode();

                    string content = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<HttpResponseModel>(content, new HttpResponseJsonConverter(_parsingStrategies));

                    return result;
                }
            }
            catch (HttpRequestException)
            {
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
