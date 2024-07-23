using joblist.Model;
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
        public DbSet<Project> project { get; set; }
        public DbSet<Rapat> rapat { get; set; }
        public DbSet<Unit> unit { get; set; }
        public DbSet<Joblist> joblist { get; set; }
        public DbSet<Equipments> equipments { get; set; }
        public DbSet<CatalogProfile> catalogProfile { get; set; }
        public DbSet<ViewReadinessEquipment> view_readiness_equipment { get; set; }
        public DbSet<ViewReadinessDetail> view_readiness_detail { get; set; }
        public DbSet<ViewGrafikReadiness> view_grafik_readiness { get; set; }
    }
}


