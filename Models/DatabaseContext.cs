using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using mystap.Models;
namespace mystap.Models
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) {
        }

        public DbSet<Plans> plans { get; set; }
        public DbSet<Users> users { get; set; }
        public DbSet<ViewReadinessEquipment> view_readiness_equipment { get; set; }
    }
}


