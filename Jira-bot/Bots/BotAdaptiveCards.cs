using AdaptiveCards;
using Microsoft.Bot.Schema;

namespace Jira_bot.Bots
{
    ///<summary>
    /// This class is responsible to create adaptive card templates
    /// </summary>

    public class BotAdaptiveCards
    {
        /// <summary>
        /// This method creates source adaptive card
        /// </summary>
        /// <returns>Source adaptive card template</returns>
        public Attachment CreateSourceAdaptiveCard()
        {
            var card = new AdaptiveCard(new AdaptiveSchemaVersion(1, 0))
            {
                Body =
                {
                    new AdaptiveTextBlock
                    {
                        Text = "Please enter your source details:",
                        Size = AdaptiveTextSize.Large,
                        Weight = AdaptiveTextWeight.Bolder
                    },
                    new AdaptiveTextInput
                    {
                        Label = "Email",
                        Id = "Email",
                        Placeholder = "Enter your email",
                        IsRequired = true,
                        ErrorMessage = "Required user email",
                    },
                     new AdaptiveTextInput
                    {
                         Label = "Source token"
        ,                Id = "SourceToken",
                        Placeholder = "Enter your source token",
                        IsRequired = true,
                        ErrorMessage = "Required source token"
                    },
                    new AdaptiveTextInput
                    {
                        Label = "Source Url",
                        Id = "SourceURL",
                        Placeholder = "Enter your source url",
                                                IsRequired = true,
                        ErrorMessage = "Required source url"
                    }
                },
                Actions =
                {
                    new AdaptiveSubmitAction
                    {
                        Title = "Submit Source Details",
                        DataJson = "{ \"type\": \"submit\",\"cardType\": \"sourceCard\" }"
                    }
                }
            };

            return new Attachment
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card
            };
        }

        /// <summary>
        /// This method is responsible for creating worklog adaptive card
        /// </summary>
        /// <returns>Worklog adaptive card template</returns>
        public Attachment CreateWorklogAdaptiveCard()
        {
            var card = new AdaptiveCard(new AdaptiveSchemaVersion(1, 0))
            {
                Body =
                {
                    new AdaptiveTextBlock
                    {
                        Text = "Please enter your worklog details:",
                        Size = AdaptiveTextSize.Large,
                        Weight = AdaptiveTextWeight.Bolder
                    },
                    new AdaptiveDateInput
                    {
                        Label = "Enter worklog date",
                        Id = "Date",
                        IsRequired = true,
                        ErrorMessage = "Required worklog date"
                    },
                    new AdaptiveTimeInput
                    {
                        Label = "Enter worklog time",
                        Id="Time",
                        IsRequired = true,
                        ErrorMessage = "Required worklog time"
                    },
                     new AdaptiveTextInput
                    {
                        Label = "Issue Number",
                        Id = "IssueNumber",
                        Placeholder = "Enter your issue number",
                        IsRequired = true,
                        ErrorMessage = "Required issue number"
                    },
                      new AdaptiveTextInput
                    {
                        Label = "Worklog Description",
                        Id = "WorklogDescription",
                        Placeholder = "Enter worklog description"
                    }
                },
                Actions =
                {
                    new AdaptiveSubmitAction
                    {
                        Title = "Submit Worklog",
                        DataJson = "{ \"type\": \"submit\", \"cardType\": \"worklogCard\" }"
                    }
                }
            };

            return new Attachment
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card
            };
        }
    }
}
