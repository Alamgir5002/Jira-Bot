using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using FromBodyAttribute = Microsoft.Azure.Functions.Worker.Http.FromBodyAttribute;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Net;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DotNetTraining.TeamsBot
{
    public class DoWorklog
    {
        private readonly ILogger<DoWorklog> _logger;

        public DoWorklog(ILogger<DoWorklog> logger)
        {
            _logger = logger;
        }

        [Function("DoWorklog")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req,
            [FromBody] JiraWorklogOptions options)
        {
            try
            {
                var worklogDoc = await AddWorklogOnJiraAsync(options);

                var response = req.CreateResponse(worklogDoc.StatusCode);
                response.Headers.Add("Content-Type", "application/json");
                await response.WriteStringAsync(JsonConvert.SerializeObject(new
                {
                    message = "Worklog created successfully!",
                }), Encoding.UTF8);

                var multiResponseDoc = new CosmosDbRecordDocument
                {
                    id = Guid.NewGuid().ToString(),
                    email = options.email,
                    baseUrl = options.baseUrl,
                    issueId = options.issueId,
                    WorklogBody = options.body,
                    jiraRequestStatus = worklogDoc.StatusCode
                };

                var multiResponse = new MultiResponse()
                {
                    Document = multiResponseDoc,
                    response = worklogDoc.StatusCode
                };

                return new OkObjectResult(multiResponse);
            }
            catch (Exception ex)
            {

                return new BadRequestObjectResult(ex.Message);
            }
            
        }

        public async Task<HttpResponseMessage> AddWorklogOnJiraAsync(JiraWorklogOptions options)
        {
            var authString = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{options.email}:{options.token}"));
            var url = $"{options.baseUrl}/rest/api/3/issue/{options.issueId}/worklog";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authString);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var content = new StringContent(JsonConvert.SerializeObject(options.body), Encoding.UTF8, "application/json");

                var response = await client.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError(response.ReasonPhrase);
                    throw new Exception($"Failed to add worklog: {response.StatusCode} \n {response.ReasonPhrase}");
                }

                return response;
            }
        }

    }

    public class MultiResponse
    {
        [CosmosDBOutput("teams-bot-db", "teams-bot-container", Connection = "CosmosDbConnectionSetting")]
        public CosmosDbRecordDocument? Document { get; set; }
        public HttpStatusCode response { get; set; }
    }

    public class CosmosDbRecordDocument
    {
        public string? id { get; set; }
        public string? email { get; set; }
        public string? baseUrl { get; set; }
        public string? issueId { get; set; }
        public JiraWorklogBody? WorklogBody { get; set; }
        public HttpStatusCode jiraRequestStatus { get; set; }
    }
    public class MyDocument
    {
        public string? id { get; set; }
        public string? message { get; set; }
    }

    public record JiraWorklogOptions(string baseUrl, string email, string token, string issueId, JiraWorklogBody body);
    public record JiraWorklogBody(string started, long? timeSpentSeconds, string? timeSpent, CommentRecord? comment);
    public record CommentRecord(
        string type,
        int version,
        List<ContentRecord> content
    );

    public record ContentRecord(
        string type,
        List<TextContentRecord> content
    );

    public record TextContentRecord(
        string text,
        string type = "text"
    );
}
