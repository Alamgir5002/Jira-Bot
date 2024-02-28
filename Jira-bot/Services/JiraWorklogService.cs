using Jira_bot.Interfaces;
using Jira_bot.Models;
using Jira_bot.Repository;
using Jira_bot.Repository.Interfaces;

namespace Jira_bot.Services
{
    public class JiraWorklogService : IJiraWorklogService
    {
        private ISourceDetailsRepository sourceDetailRepository;
        public JiraWorklogService(ISourceDetailsRepository sourceDetailRepository)
        {
            this.sourceDetailRepository = sourceDetailRepository;
        }
        public SourceDetails AddSourceDetails(SourceDetails sourceDetails)
        {
            return sourceDetailRepository.AddSourceDetails(sourceDetails);
        }

        public bool checkIfUserAlreadyRegistered(string userId)
        {
            SourceDetails sourceDetails = sourceDetailRepository.GetSourceDetailsFromUserId(userId);
            return sourceDetails != null;
        }
    }
}
