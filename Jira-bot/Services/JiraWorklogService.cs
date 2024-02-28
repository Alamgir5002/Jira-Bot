﻿using Jira_bot.Interfaces;
using Jira_bot.Models;
using Jira_bot.Repository.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Jira_bot.Services
{
    public class JiraWorklogService : IJiraWorklogService
    {
        private ISourceDetailsRepository sourceDetailRepository;
        private HttpClientService httpClientService;
        private ILogger<JiraWorklogService> logger;
        public JiraWorklogService(ISourceDetailsRepository sourceDetailRepository, ILogger<JiraWorklogService> logger)
        {
            this.sourceDetailRepository = sourceDetailRepository;
            httpClientService = new HttpClientService();
            this.logger = logger;   
        }
        public SourceDetails AddSourceDetails(SourceDetails sourceDetails)
        {
            return sourceDetailRepository.AddSourceDetails(sourceDetails);
        }

        public async Task<string> AddWorklogForUser(JiraWorkLog worklogDetails, string userId)
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

        private async Task<string> AddWorklogOnSource(JiraWorkLog jiraWorkLogDetails)
        {
            try
            {
                SourceDetails sourceDetails = new SourceDetails();
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(jiraWorkLogDetails);
                HttpResponseMessage responseMessage = await httpClientService.SendPostRequest("https://dotnetbreakworklogbot.azurewebsites.net/api/DoWorklog", json);
                logger.LogDebug($"Response from azure function : {responseMessage.Content.ToString()}");

                if (responseMessage.IsSuccessStatusCode)
                {
                    return $"Worklog added against issue {jiraWorkLogDetails.issueId}";
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
    }
}
