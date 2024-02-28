using Microsoft.EntityFrameworkCore;

namespace Jira_bot.Models
{
    public class DatabaseContext : DbContext
    {
        public DbSet<SourceDetails> SourceDetails { get; set; }
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }
    }
}
