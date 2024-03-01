using Microsoft.EntityFrameworkCore;

namespace Jira_teams_bot.Models
{
    public class DatabaseContext : DbContext
    {
        public DbSet<SourceDetails> SourceDetails { get; set; }
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }
    }
}
