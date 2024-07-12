using Microsoft.EntityFrameworkCore;
namespace mystap.Models
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) {
        }

        public DbSet<Plans> plans { get; set; }
        public DbSet<Users> users { get; set; }
    }
}
