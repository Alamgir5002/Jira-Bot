using Jira_bot.Models;

namespace Jira_bot.Repository.Interfaces
{
    public interface ISourceDetailsRepository
    {
        public SourceDetails AddSourceDetails(SourceDetails sourceDetails);
        public SourceDetails GetSourceDetailsFromUserId(string userId);
    }
}
