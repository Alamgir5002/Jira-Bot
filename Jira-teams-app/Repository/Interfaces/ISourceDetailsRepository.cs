using Jira_teams_bot.Models;

namespace Jira_teams_bot.Repository.Interfaces
{
    public interface ISourceDetailsRepository
    {
        public SourceDetails AddSourceDetails(SourceDetails sourceDetails);
        public SourceDetails GetSourceDetailsFromUserId(string userId);
    }
}
