﻿using Jira_bot.Models;
using System.Threading.Tasks;

namespace Jira_bot.Interfaces
{
    public interface IJiraWorklogService
    {
        public SourceDetails AddSourceDetails(SourceDetails sourceDetails);
        public bool checkIfUserAlreadyRegistered(string userId);
        public Task<string> AddWorklog(JiraWorkLog worklogDetails);
    }
}
