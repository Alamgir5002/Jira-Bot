using System;

namespace Jira_bot.Exceptions
{
    public class SourceDetailsException: Exception
    {
        public SourceDetailsException(string exceptionMessage) : base(exceptionMessage) { }
    }
}
