using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Polly;
using Polly.Retry;
using Polly.Timeout;
using ToDoListApp.Helpers;
using ToDoListApp.Models;

namespace ToDoListApp.Repository
{
    public class RestService : IRestService
    {
        private HttpClient _httpClient;
        private readonly Uri _baseUri = new Uri(ConstantValues.BaseUri);
        private readonly string _developerName = ConstantValues.DeveloperName;

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
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var stream = await response.Content.ReadAsStreamAsync();
                        HttpResponseModel result = stream.ReadAndDeserializeFromJson<HttpResponseModel>();
                        
                        return result;
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
            }
            catch (OperationCanceledException)
            {
                return null;
            }
            catch (HttpRequestException ex)
            {
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {

            }
        }

        public async Task<HttpResponseModel> LoginAsync(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException(nameof(userName));

            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException(nameof(password));

            var builder = new UriBuilderExt(string.Concat(_baseUri, ConstantValues.LoginUri));
            //builder.AddParameter("developer", _developerName);

            var uri = builder.Uri;

            try
            {
                using (var formData = new MultipartFormDataContent())
                {
                    formData.Add(new StringContent(userName), "\"username\"");
                    formData.Add(new StringContent(password), "\"password\"");

                    AsyncRetryPolicy<HttpResponseMessage> httpRetryPolicy =
                        Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                            .Or<TimeoutRejectedException>()
                            .RetryAsync(2);

                    AsyncTimeoutPolicy timeoutPolicy = Policy.TimeoutAsync(ConstantValues.HttpRequestDefaultTimeout);

                    HttpResponseMessage response = await
                        httpRetryPolicy.ExecuteAsync(() =>
                            timeoutPolicy.ExecuteAsync(async token =>
                                await _httpClient.PostAsync(uri, formData, token), CancellationToken.None));

                    response.EnsureSuccessStatusCode();

                    string content = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<HttpResponseModel>(content);

                    return result;
                }
            }
            catch (TimeoutRejectedException)
            {
                // TODO
                return null;
            }
            catch (HttpRequestException ex)
            {
                // TODO
                //return new LoginModel(ex.Message, null);
                return null;
            }
            finally
            {

            }
        }
    }
}
