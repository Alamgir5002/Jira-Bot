using System;
using System.Collections.Generic;

namespace Jira_bot.Models
{
    public class SourceWorkLog
    {
        public string baseUrl { get; set; }
        public string email { get; set; }
        public string token { get; set; }
        public string issueId { get; set; }
        public WorklogDetailsBody body {set; get; }
    }
    public class WorklogDetailsBody
    {
        public string started {get; set; }
        public int timeSpentSeconds { get; set;}
        public CommentRecord? comment { get; set;}

    }

    public record CommentRecord(
        List<ContentRecord> content,
        string type = "doc",
        int version = 1
    );

    public record ContentRecord(
        List<TextContentRecord> content,
        string type = "paragraph"
    );

    public record TextContentRecord(
        string text,
        string type = "text"
    );
}
