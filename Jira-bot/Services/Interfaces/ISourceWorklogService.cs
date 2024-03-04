using Jira_bot.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Threading;
using System.Threading.Tasks;

namespace Jira_bot.Interfaces
{
    public interface ISourceWorklogService
    {
        public  Task AddSourceDetails(SourceDetails sourceDetails, ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken);
        public bool checkIfUserAlreadyRegistered(string userId);
        public Task<string> AddWorklogForUser(SourceWorkLog worklogDetails, string userId);
    }
}
