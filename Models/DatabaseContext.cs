
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
namespace mystap.Models
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) {
        }

        //Core Model
        public DbSet<Plans> plans { get; set; }
        public DbSet<Users> users { get; set; }
        public DbSet<Project> project { get; set; }
        public DbSet<Rapat> rapat { get; set; }
        public DbSet<Steerco> steerco { get; set; }
        public DbSet<Unit> unit { get; set; }
        public DbSet<UnitProses> unitProses { get; set; }
        public DbSet<Equipments> equipments { get; set; }
        public DbSet<CatalogProfile> catalogProfile { get; set; }
        public DbSet<Disiplin> disiplins { get; set; }
        public DbSet<Joblist> joblist { get; set; }
        public DbSet<Joblist_Detail> joblist_Detail { get; set; }
        public DbSet<JoblistDetailMemo> joblistDetailMemo { get; set; }
        public DbSet<ContractTracking> contractTracking { get; set; }
        public DbSet<Durasi> durasi { get; set; }
        public DbSet<Memo> memo { get; set; }
        public DbSet<Notifikasi> notifikasi { get; set; }
        public DbSet<Requestor> requestors { get; set; }
        public DbSet<Sow> sow { get; set; }
        public DbSet<SowGroup> sowGroup { get; set; }
        public DbSet<HistoryReservasi> historyReservasi { get; set; }
        public DbSet<Zpm01>zpm01 { get; set; }
        public DbSet<Bom>bom { get; set; }
        public DbSet<BomFiles> bomFiles { get; set; }



        //View Model
        public DbSet<ViewReadinessEquipment> view_readiness_equipment { get; set; }
        public DbSet<ViewReadinessDetail> view_readiness_detail { get; set; }
        public DbSet<ViewGrafikReadiness> view_grafik_readiness { get; set; }
        public DbSet<ViewDetailOrder> view_detail_order { get; set; }
        public DbSet<ViewReadinessJoblist> view_readiness_joblist { get; set; }
        public DbSet<ViewCountSummaryMaterial> view_count_summary_material { get; set; }
        public DbSet<ViewDetailSummaryMaterial> view_detail_summary_material { get; set; }
        public DbSet<ViewGrafikMaterial> view_grafik_material { get; set; }
        public DbSet<ViewSummaryMaterial> view_summary_material { get; set; }
        public DbSet<ViewDetailMaterial> view_detail_material { get; set;}
        public DbSet<ViewJoblist> view_joblist { get; set; }
    }
}


