using System.Text;


namespace Jira_teams_bot.Services
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
