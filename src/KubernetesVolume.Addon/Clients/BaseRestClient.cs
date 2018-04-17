namespace KubernetesVolume.Addon.Clients
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using RestSharp;

    internal abstract class BaseRestClient : RestClient
    {
        protected BaseRestClient(string baseUrl)
            : base(baseUrl)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ClearHandlers();
            AddHandler("application/json", new JsonDeserializer());
        }

        protected virtual async Task<IRestResponse<T>> ExecRequestAsync<T>(Method method, string restUrl, object body = null)
        {
            var response = await GetResponseAsync<T>(method, restUrl, body);

            ValidateResponse(response);

            return response;
        }

        protected async Task<IRestResponse<T>> GetResponseAsync<T>(Method method, string restUrl, object body = null)
        {
            var request = new RestRequest(restUrl, method)
            {
                RequestFormat = DataFormat.Json,
                JsonSerializer = new JsonSerializer()
            };

            if (body != null)
            {
                request.AddBody(body);
            }

            return await ExecuteTaskAsync<T>(request);
        }

        protected void ValidateResponse<T>(IRestResponse<T> response)
        {
            if (response.StatusCode == 0)
            {
                throw new ApplicationException("Connection failed.");
            }

            if ((int)response.StatusCode >= 400 && response.StatusCode != HttpStatusCode.NotFound)
            {
                throw new ApplicationException($"Request failed with status code {response.StatusCode}: {response.Content}");
            }
        }
    }
}
