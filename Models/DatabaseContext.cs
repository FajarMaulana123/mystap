﻿
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
namespace mystap.Models
{
    public class DatabaseContext : IdentityDbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) {
        }

        //Core Model
        public DbSet<Plans> plans { get; set; }
        public DbSet<Users> users { get; set; }
        public DbSet<UserModul> userModul { get; set; }
        public DbSet<Modul> modul { get; set; }
		public DbSet<Project> project { get; set; }
        public DbSet<Rapat> rapat { get; set; }
        public DbSet<Steerco> steerco { get; set; }
        public DbSet<Pir> pir { get; set; }
        public DbSet<Unit> unit { get; set; }
        public DbSet<UnitProses> unitProses { get; set; }
        public DbSet<Equipments> equipments { get; set; }
        public DbSet<CatalogProfile> catalogProfile { get; set; }
        public DbSet<Disiplin> disiplins { get; set; }
        public DbSet<Joblist> joblist { get; set; }
        public DbSet<Joblist_Detail> joblist_Detail { get; set; }
        public DbSet<JoblistDetailMemo> joblistDetailMemo { get; set; }
        public DbSet<JoblistDetailWo> joblistDetailWo { get; set; }
        public DbSet<PaketJoblist> paketJoblist { get; set; }
        public DbSet<ContractTracking> contractTracking { get; set; }
        public DbSet<Durasi> durasi { get; set; }
        public DbSet<Memo> memo { get; set; }
        public DbSet<Notifikasi> notifikasi { get; set; }
        public DbSet<FungsiBagian> fungsiBagian { get; set; }
        public DbSet<Requestor> requestors { get; set; }
        public DbSet<Sow> sow { get; set; }
        public DbSet<SowGroup> sowGroup { get; set; }
        public DbSet<HistoryReservasi> historyReservasi { get; set; }
        public DbSet<Zpm01>zpm01 { get; set; }
        public DbSet<Zpm02> zpm02 { get; set; }
        public DbSet<WorkOrder>work_order { get; set; }
        public DbSet<PurchOrder>purch_order { get; set; }
        public DbSet<Bom>bom { get; set; }
        public DbSet<BomFiles> bomFiles { get; set; }
        public DbSet<ContractItem> contractItem { get; set; }


        //View Model
        public DbSet<ViewBom> viewBom { get; set; }
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
        public DbSet<ViewPlanningJoblist> viewPlanningJoblist { get; set; }
        public DbSet<ViewPaketJoblist> viewPaketJoblist { get; set; }
        public DbSet<ViewCountEksekusi> viewCountEksekusi { get; set; }
        public DbSet<ViewReservasi> viewReservasi { get; set; }
        public DbSet<ViewOutstandingReservasi> viewOutstandingReservasi { get; set; }
        public DbSet<ViewUpdatePr> viewUpdatePr { get; set; }
        public DbSet<ViewListPr> viewListPr { get; set; }
        public DbSet<ViewDistribusiPr> viewDistribusiPr { get; set; }
    }
}


