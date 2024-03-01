﻿using Jira_teams_bot.Models;
using System.Threading.Tasks;

namespace Jira_teams_bot.Interfaces
{
    public interface IJSourceWorklogService
    {
        public SourceDetails AddSourceDetails(SourceDetails sourceDetails);
        public bool checkIfUserAlreadyRegistered(string userId);
        public Task<string> AddWorklogForUser(SourceWorkLog worklogDetails, string userId);
    }
}
