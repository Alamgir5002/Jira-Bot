using Jira_bot.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
    }
}
