using Jira_bot.Exceptions;
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
    /// <summary>
    /// This class implements SourceWorklogService interface
    /// </summary>
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

        /// <summary>
        /// This method is responsible to add source details in database
        /// </summary>
        /// <param name="sourceDetails">Source Details</param>
        /// <param name="turnContext">Turn Context</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns></returns>
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
                logger.LogError($"Exception occurred: {exception.Message}");
                await turnContext.SendActivityAsync(MessageFactory.Text(exception.Message, exception.Message), cancellationToken);
                await ShowAddSourceCredentialsCard(turnContext, cancellationToken);
            }
        }

        /// <summary>
        /// This method is responsible to add worklog for user
        /// </summary>
        /// <param name="worklogDetails">Worklog Details</param>
        /// <param name="userId">User Id</param>
        /// <returns></returns>
        public async Task<string> AddWorklogForUser(SourceWorkLog worklogDetails, string userId)
        {
            SourceDetails sourceDetails = sourceDetailRepository.GetSourceDetailsFromUserId(userId);
            
            if(sourceDetails == null)
            {
                logger.LogDebug($"Source details not found against user with id {userId}");
                throw new SourceDetailsException("Can't log time on source because your source details are not present");
            }

            worklogDetails.email = sourceDetails.UserEmail;
            worklogDetails.token = sourceDetails.SourceToken;
            worklogDetails.baseUrl = sourceDetails.SourceURL;

            return await addWorklogOnSource(worklogDetails);
        }

        /// <summary>
        /// This method calls azure function which is responsible for adding worklog on source
        /// </summary>
        /// <param name="sourceWorkLogDetails">worklog details</param>
        /// <returns>Returns success/failure message</returns>
        private async Task<string> addWorklogOnSource(SourceWorkLog sourceWorkLogDetails)
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

                return "Error occurred while adding worklog on JIRA, either issue not found or you don't have access to this issue.";
            }
            catch (Exception ex)
            {
                logger.LogDebug($"Exception : {ex.Message}");
                return ex.Message;
            }
        }
        /// <summary>
        /// Method to check if user already exist against given id
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <returns>boolean value to check if user already exist in database or not.</returns>

        public bool checkIfUserAlreadyRegistered(string userId)
        {
            SourceDetails sourceDetails = sourceDetailRepository.GetSourceDetailsFromUserId(userId);
            return sourceDetails != null;
        }

        /// <summary>
        /// Method responsible to validate source URL by calling source API call.
        /// </summary>
        /// <param name="sourceDetails">source details</param>
        /// <exception cref="Exception">Throws exception if souce URL is invalid or not found on source.</exception>
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

        /// <summary>
        /// Method responsible to validate source details by calling API on source.
        /// </summary>
        /// <param name="sourceDetails">Source Details</param>
        /// <exception cref="Exception">Throws exception if token or username are invalid.</exception>

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
