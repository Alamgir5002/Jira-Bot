using Jira_bot.Interfaces;
using Jira_bot.Models;
using Jira_bot.Repository.Interfaces;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Jira_bot.Services
{
    public class SourceWorklogService : ISourceWorklogService
    {
        private ISourceDetailsRepository sourceDetailRepository;
        private HttpClientService httpClientService;
        private ILogger<ISourceWorklogService> logger;

        private const string JIRA_INSTANCE_INFO_URL = "/rest/api/3/serverInfo";
        private const string JIRA_USER_CREDENTIALS_INFO_URL = "/rest/api/3/myself";
        public SourceWorklogService(ISourceDetailsRepository sourceDetailRepository, ILogger<ISourceWorklogService> logger)
        {
            this.sourceDetailRepository = sourceDetailRepository;
            httpClientService = new HttpClientService();
            this.logger = logger;   
        }
        public async Task AddSourceDetails(SourceDetails sourceDetails, ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            try
            {
                await validateSourceURL(sourceDetails);
                await validateSourceDetails(sourceDetails);

                sourceDetailRepository.AddSourceDetails(sourceDetails);

                var replyText = "Source details added successfully. Now you can log your work using log command";
                await turnContext.SendActivityAsync(MessageFactory.Text(replyText, replyText), cancellationToken);
            }
            catch(Exception exception)
            {
                await turnContext.SendActivityAsync(MessageFactory.Text(exception.Message, exception.Message), cancellationToken);
            }
        }

        public async Task<string> AddWorklogForUser(SourceWorkLog worklogDetails, string userId)
        {
            SourceDetails sourceDetails = sourceDetailRepository.GetSourceDetailsFromUserId(userId);
            
            if(sourceDetails == null)
            {
                logger.LogDebug($"Source details not found against user with id {userId}");
                return "Can't log time on source because your source details are not present";
            }

            worklogDetails.email = sourceDetails.UserEmail;
            worklogDetails.token = sourceDetails.SourceToken;
            worklogDetails.baseUrl = sourceDetails.SourceURL;

            return await AddWorklogOnSource(worklogDetails);
        }

        private async Task<string> AddWorklogOnSource(SourceWorkLog sourceWorkLogDetails)
        {
            try
            {
                SourceDetails sourceDetails = new SourceDetails();
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(sourceWorkLogDetails);
                HttpResponseMessage responseMessage = await httpClientService.SendPostRequest("https://dotnetbreakworklogbot.azurewebsites.net/api/DoWorklog", json);
                logger.LogDebug($"Response from azure function : {responseMessage.Content.ToString()}");

                if (responseMessage.IsSuccessStatusCode)
                {
                    return $"Worklog added against issue {sourceWorkLogDetails.issueId}";
                }

                return "Error occurred";
            }
            catch (Exception ex)
            {
                logger.LogDebug($"Exception : {ex.Message}");
                return ex.Message;
            }
        }

        public bool checkIfUserAlreadyRegistered(string userId)
        {
            SourceDetails sourceDetails = sourceDetailRepository.GetSourceDetailsFromUserId(userId);
            return sourceDetails != null;
        }


        private async Task validateSourceURL(SourceDetails sourceDetails)
        {
            string url = sourceDetails.SourceURL + JIRA_INSTANCE_INFO_URL;
            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                throw new Exception($"Invalid URL : {sourceDetails.SourceURL}");
            }

            HttpResponseMessage httpResponse = await httpClientService.SendGetRequest(url);

            if (!httpResponse.IsSuccessStatusCode)
            {
                throw new Exception($"JIRA instance not found at URL : {sourceDetails.SourceURL}");
            }
        }

        private async Task validateSourceDetails(SourceDetails sourceDetails)
        {
            string basicAuthString = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{sourceDetails.UserEmail}:{sourceDetails.SourceToken}"));
            string url = sourceDetails.SourceURL + JIRA_USER_CREDENTIALS_INFO_URL;

            HttpResponseMessage httpResponse = await httpClientService.SendGetRequestWithBasicAuthHeaders(url, basicAuthString);
            if(!httpResponse.IsSuccessStatusCode)
            {
                throw new Exception("Invalid source token or user email");
            }
        }
    }
}
