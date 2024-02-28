using Microsoft.EntityFrameworkCore;

namespace Models
{
    public class DatabaseContext : DbContext
    {
        public DbSet<SourceDetails> SourceDetails { get; set; }
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }
    }
}
