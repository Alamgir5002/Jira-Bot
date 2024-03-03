using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System;
using Jira_bot.Interfaces;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Bot.Builder.Teams;
using Jira_bot.Services;

namespace Jira_bot.Bots
{

    public class EchoBot : TeamsActivityHandler
    {
        private ISourceWorklogService sourceWorklogService;
        private AdaptiveCardService adaptiveCardService;

        public EchoBot(ISourceWorklogService sourceWorklogService, AdaptiveCardService adaptiveCardService)
        {
            this.sourceWorklogService = sourceWorklogService;
            this.adaptiveCardService = adaptiveCardService;
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
                        await adaptiveCardService.processWorklogCard(turnContext, cardData, cancellationToken);
                        break;
                    default:
                        await adaptiveCardService.processSourceCard(turnContext, cardData, cancellationToken);
                        break;
                }
            }
            else if (turnContext.Activity.Text.ToLower().StartsWith("log", StringComparison.OrdinalIgnoreCase))
            {
                await adaptiveCardService.ShowAddWorklogCard(turnContext, cancellationToken);
            }
            else
            {
                var replyText = "Command not found";
                await turnContext.SendActivityAsync(MessageFactory.Text(replyText, replyText), cancellationToken);
            }
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {

            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    if (!sourceWorklogService.checkIfUserAlreadyRegistered(member.Id))
                    {
                        await adaptiveCardService.ShowAddSourceCredentialsCard(turnContext, cancellationToken);
                    }
                    else
                    {

                        var welcomeText = $"Hello and welcome !";
                        await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText, welcomeText), cancellationToken);
                    }
                }
            }
        }
    }
}
