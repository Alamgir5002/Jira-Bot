using Jira_teams_bot.Interfaces;
using Jira_teams_bot.Models;
using Jira_teams_bot.Repository;
using Jira_teams_bot.Repository.Interfaces;
using Jira_teams_bot.Services;
using Jira_teams_app;
using Jira_teams_app.Bot;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHttpClient("WebClient", client => client.Timeout = TimeSpan.FromSeconds(600));
builder.Services.AddHttpContextAccessor();

//adding sql connection
//var connectionString = "Server=tcp:banking-server-sql-server.database.windows.net,1433;Initial Catalog=Jira-teams-bot-DB;Persist Security Info=False;User ID=alamgir5002;Password=mercedes@5002;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
var connectionString = "Server=tcp:banking-server-sql-server.database.windows.net,1433;Initial Catalog=Jira-teams-bot-DB;Persist Security Info=False;User ID=alamgir5002;Password=mercedes@5002;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
builder.Services.AddDbContext<DatabaseContext>(options =>
{
    options.UseSqlServer(connectionString);
});

// Create the Bot Framework Authentication to be used with the Bot Adapter.
var config = builder.Configuration.Get<ConfigOptions>();
builder.Configuration["MicrosoftAppType"] = "MultiTenant";
builder.Configuration["MicrosoftAppId"] = config.BOT_ID;
builder.Configuration["MicrosoftAppPassword"] = config.BOT_PASSWORD;
builder.Services.AddSingleton<BotFrameworkAuthentication, ConfigurationBotFrameworkAuthentication>();

// Create the Bot Framework Adapter with error handling enabled.
builder.Services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

// Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
builder.Services.AddTransient<IBot, EchoBot>();

builder.Services.AddScoped<ISourceDetailsRepository, SourceDetailsRepository>();
builder.Services.AddScoped<IJSourceWorklogService, SourceWorklogService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();