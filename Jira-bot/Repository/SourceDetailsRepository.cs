using Jira_bot.Models;
using Jira_bot.Repository.Interfaces;
using System.Linq;

namespace Jira_bot.Repository
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
