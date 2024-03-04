using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Jira_bot.Services
{
    public class HttpClientService
    {
        private HttpClient httpClient;
        public HttpClientService()
        {
            httpClient = new HttpClient();
        }

        public async Task<HttpResponseMessage> SendPostRequest(string endpoint, string jsonData)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, endpoint);
            request.Headers.Add("Accept", "application/json");
            request.Content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            return await httpClient.SendAsync(request);
        }

        public async Task<HttpResponseMessage> SendGetRequest(string endpoint)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, endpoint);
            request.Headers.Add("Accept", "application/json");
            return await httpClient.SendAsync(request);
        }

        public async Task<HttpResponseMessage> SendGetRequestWithBasicAuthHeaders(string endpoint, string basicAuthString)
        {
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", basicAuthString);
            return await httpClient.GetAsync(endpoint);

        }
    }
}
