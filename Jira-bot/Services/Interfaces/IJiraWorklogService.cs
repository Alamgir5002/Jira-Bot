using Jira_bot.Models;

namespace Jira_bot.Interfaces
{
    public interface IJiraWorklogService
    {
        public SourceDetails AddSourceDetails(SourceDetails sourceDetails);
        public bool checkIfUserAlreadyRegistered(string userId);
    }
}
