using Jira_bot.Interfaces;
using Jira_bot.Models;
using Jira_bot.Repository.Interfaces;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Jira_bot.Services
{
    public class JiraWorklogService : IJiraWorklogService
    {
        private ISourceDetailsRepository sourceDetailRepository;
        private HttpClientService httpClientService;
        public JiraWorklogService(ISourceDetailsRepository sourceDetailRepository)
        {
            this.sourceDetailRepository = sourceDetailRepository;
            httpClientService = new HttpClientService();
        }
        public SourceDetails AddSourceDetails(SourceDetails sourceDetails)
        {
            return sourceDetailRepository.AddSourceDetails(sourceDetails);
        }

        public async Task<string> AddWorklog(JiraWorkLog worklogDetails)
        {
            try
            {
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(worklogDetails);
                HttpResponseMessage responseMessage = await httpClientService.SendPostRequest("https://dotnetbreakworklogbot.azurewebsites.net/api/DoWorklog", json);
                if(responseMessage.IsSuccessStatusCode)
                {
                    return $"Worklog added against issue {worklogDetails.issueId}";
                }

                Console.WriteLine("Response:", responseMessage);
                return await responseMessage.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
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
