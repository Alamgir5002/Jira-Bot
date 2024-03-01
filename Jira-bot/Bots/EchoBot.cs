using Jira_teams_bot.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System;
using AdaptiveCards;
using Jira_teams_bot.Interfaces;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Bot.Builder.Teams;

namespace Jira_teams_bot.Bots
{

    public class EchoBot : TeamsActivityHandler
    {
        private IJSourceWorklogService sourceWorklogService;
        private ILogger<EchoBot> logger;

        public EchoBot(IJSourceWorklogService sourceWorklogService, ILogger<EchoBot> logger)
        {
            this.sourceWorklogService = sourceWorklogService;
            this.logger = logger;
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {

            if (turnContext.Activity.Type == ActivityTypes.Message && turnContext.Activity.Value != null)
            {
                var cardData = JObject.FromObject(turnContext.Activity.Value);
                var cardType = cardData["cardType"]?.ToString();

                switch (cardType)
                {
                    case "worklogCard":
                        await processWorklogCard(turnContext, cardData, cancellationToken);
                        break;
                    default:
                        await processSourceCard(turnContext, cardData, cancellationToken);
                        break;
                }
            }
            else if (turnContext.Activity.Text.ToLower().StartsWith("log", StringComparison.OrdinalIgnoreCase))
            {
                var cardAttachment = CreateWorklogAdaptiveCard();
                var reply = MessageFactory.Attachment(cardAttachment);
                await turnContext.SendActivityAsync(reply, cancellationToken);
            }
            else
            {
                var replyText = "Not found";
                await turnContext.SendActivityAsync(MessageFactory.Text(replyText, replyText), cancellationToken);
            }
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {

            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {;
                    if (!sourceWorklogService.checkIfUserAlreadyRegistered(member.Id))
                    {
                        var cardAttachment = CreateSourceAdaptiveCard();
                        var reply = MessageFactory.Attachment(cardAttachment);
                        await turnContext.SendActivityAsync(reply, cancellationToken);
                    }
                    else
                    {

                        var welcomeText = $"Hello and welcome !";
                        await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText, welcomeText), cancellationToken);
                    }
                }
            }
        }

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


        private async Task processSourceCard(ITurnContext<IMessageActivity> turnContext, JObject jsonData, CancellationToken cancellationToken)
        {
            var sourceDetails = new SourceDetails()
            {
                SourceToken = (string)jsonData["SourceToken"],
                UserEmail = (string)jsonData["Email"],
                SourceURL = (string)jsonData["SourceURL"],
                Id = turnContext.Activity.From.Id
            };
            sourceWorklogService.AddSourceDetails(sourceDetails);
            var replyText = "Source details added successfully. Now you can log your work using log command";
            await turnContext.SendActivityAsync(MessageFactory.Text(replyText, replyText), cancellationToken);
        }

        private async Task processWorklogCard(ITurnContext<IMessageActivity> turnContext, JObject jsonData, CancellationToken cancellationToken)
        {
            var worklogDetails = new SourceWorkLog
            {
                issueId = (string)jsonData["IssueNumber"],
                body = new WorklogDetailsBody
                {
                    started = ((DateTime)jsonData["Date"]).ToString("yyyy-MM-ddTHH:mm:ss.fffzzss"),
                    timeSpentSeconds = (int)((TimeSpan)jsonData["Time"]).TotalSeconds
                }
            };

            var replyText = await sourceWorklogService.AddWorklogForUser(worklogDetails, turnContext.Activity.From.Id);
            logger.LogDebug("Reply Text:"+replyText);
            await turnContext.SendActivityAsync(MessageFactory.Text(replyText, replyText), cancellationToken);
        }

    }


}
