using Jira_teams_bot.Models;
using Jira_teams_bot.Repository.Interfaces;

namespace Jira_teams_bot.Repository
{
    public class SourceDetailsRepository : ISourceDetailsRepository
    {
        private readonly DatabaseContext databaseContext;
        public SourceDetailsRepository(DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        public SourceDetails AddSourceDetails(SourceDetails sourceDetails)
        {
            databaseContext.SourceDetails.Add(sourceDetails);
            databaseContext.SaveChanges();
            return sourceDetails;
        }

        public SourceDetails GetSourceDetailsFromUserId(string userId)
        {
            return databaseContext.SourceDetails.FirstOrDefault(sourceDetails => sourceDetails.Id == userId);
        }
    }
}
