using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;
using mystap.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddMemoryCache();
builder.Services.AddSession();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseSession();
app.UseEndpoints(endpoints =>
{

    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Auth}/{action=Login}/{id?}");

	endpoints.MapControllerRoute(
		name: "login",
		pattern: "login",
		defaults: new { controller = "Auth", action = "Login" });

	endpoints.MapControllerRoute(
		name: "logout",
		pattern: "logout",
		defaults: new { controller = "Auth", action = "Logout" });

	endpoints.MapControllerRoute(
		name: "validate_auth",
		pattern: "validate_auth",
		defaults: new { controller = "Auth", action = "ValidateLogin" });

	endpoints.MapControllerRoute(
		name: "awal",
		pattern: "awal",
		defaults: new { controller = "Awal", action = "Awal" });

    endpoints.MapControllerRoute(
        name: "profile_",
        pattern: "profile_",
        defaults: new { controller = "Data", action = "Profile_" });

    //DASHBOARD EQUIPMENT
    endpoints.MapControllerRoute(
        name: "dashboard_equipment",
        pattern: "dashboard_equipment",
        defaults: new { controller = "Dashboard", action = "DashboardEquipment" });

    endpoints.MapControllerRoute(
        name: "get_readiness_equipment",
        pattern: "get_readiness_equipment",
        defaults: new { controller = "Dashboard", action = "GetReadinessEquipment" });

    endpoints.MapControllerRoute(
        name: "grafik_readiness_equipment",
        pattern: "grafik_readiness_equipment",
        defaults: new { controller = "Dashboard", action = "GrafikReadinessEquipment" });

    endpoints.MapControllerRoute(
        name: "get_equipments",
        pattern: "get_equipments",
        defaults: new { controller = "Dashboard", action = "GetEquipment" });

    endpoints.MapControllerRoute(
        name: "readiness_detail",
        pattern: "readiness_detail/{projectID?}/{rev?}/{joblist?}",
        defaults: new { controller = "Dashboard", action = "ReadinessDetail" });

    endpoints.MapControllerRoute(
        name: "order_",
        pattern: "order_",
        defaults: new { controller = "Dashboard", action = "OrderDetail" });

    endpoints.MapControllerRoute(
        name: "detail_jasa",
        pattern: "detail_jasa",
        defaults: new { controller = "Dashboard", action = "DetailJasa" });

    //DASHBOARD JOBLIST
    endpoints.MapControllerRoute(
        name: "dashboard_joblist",
        pattern: "dashboard_joblist",
        defaults: new { controller = "Dashboard", action = "DashboardJoblist" });

    endpoints.MapControllerRoute(
        name: "get_readiness_joblist",
        pattern: "get_readiness_joblist",
        defaults: new { controller = "Dashboard", action = "GetReadinessJoblist" });

    endpoints.MapControllerRoute(
        name: "readiness_detail_joblist",
        pattern: "readiness_detail_joblist/{projectID?}/{rev?}/{paketJoblist?}",
        defaults: new { controller = "Dashboard", action = "ReadinessDetailJoblist" });

    endpoints.MapControllerRoute(
        name: "grafik_readiness_joblist",
        pattern: "grafik_readiness_joblist",
        defaults: new { controller = "Dashboard", action = "GrafikReadinessJoblist" });

    //Dashboard Jobplan
    endpoints.MapControllerRoute(
        name: "dashboard_jobplan",
        pattern: "dashboard_jobplan",
        defaults: new { controller = "Dashboard", action = "DashboardJobplan" });

    endpoints.MapControllerRoute(
        name: "data_material",
        pattern: "data_material",
        defaults: new { controller = "Dashboard", action = "SummaryMaterial" });

    endpoints.MapControllerRoute(
        name: "data_jasa",
        pattern: "data_jasa",
        defaults: new { controller = "Dashboard", action = "SummaryJasa" });

    endpoints.MapControllerRoute(
        name: "grafik_jobplan",
        pattern: "grafik_jobplan",
        defaults: new { controller = "Dashboard", action = "GrafikJobplan" });

    //Dashboard Kontrak
    endpoints.MapControllerRoute(
        name: "dashboard_kontrak",
        pattern: "dashboard_kontrak",
        defaults: new { controller = "Dashboard", action = "DashboardKontrak" });

    endpoints.MapControllerRoute(
        name: "kontrak_by_unit",
        pattern: "kontrak_by_unit",
        defaults: new { controller = "Dashboard", action = "KontrakByUnit" });

    endpoints.MapControllerRoute(
        name: "kontrak_by_status",
        pattern: "kontrak_by_status",
        defaults: new { controller = "Dashboard", action = "KontrakByStatus" });

    endpoints.MapControllerRoute(
        name: "chart_kontrak_status",
        pattern: "chart_kontrak_status",
        defaults: new { controller = "Dashboard", action = "ChartKontrakByStatus" });

    endpoints.MapControllerRoute(
        name: "count_progress_jasa",
        pattern: "count_progress_jasa",
        defaults: new { controller = "Dashboard", action = "CountProgressJasa" });

    //Dashboard Material
    endpoints.MapControllerRoute(
       name: "dashboard_material",
       pattern: "dashboard_material",
       defaults: new { controller = "Dashboard", action = "DashboardMaterial" });

    endpoints.MapControllerRoute(
       name: "grafik_status_summary",
       pattern: "grafik_status_summary",
       defaults: new { controller = "Dashboard", action = "CountSummaryMaterial" });

    endpoints.MapControllerRoute(
        name: "detail_summary_material",
        pattern: "detail_summary_material/{rev?}/{filter?}/{lldi?}",
        defaults: new { controller = "Dashboard", action = "DetailSummaryMaterial" });

    endpoints.MapControllerRoute(
       name: "detail_summary_material_",
       pattern: "detail_summary_material_",
       defaults: new { controller = "Dashboard", action = "DetailSummaryMaterial_" });

    endpoints.MapControllerRoute(
       name: "grafik_material",
       pattern: "grafik_material",
       defaults: new { controller = "Dashboard", action = "GrafikMaterial" });

    endpoints.MapControllerRoute(
       name: "grafik_progress_material",
       pattern: "grafik_progress_material",
       defaults: new { controller = "Dashboard", action = "GrafikProgresMaterial" });

    endpoints.MapControllerRoute(
       name: "summary_material_",
       pattern: "summary_material_",
       defaults: new { controller = "Dashboard", action = "SummaryMaterial_" });

    endpoints.MapControllerRoute(
       name: "detail_summary_material_pengadaan",
       pattern: "detail_summary_material_pengadaan/{rev?}/{filter?}/{lldi?}",
       defaults: new { controller = "Dashboard", action = "DetailMaterial" });

    endpoints.MapControllerRoute(
       name: "detail_summary_material_pengadaan_",
       pattern: "detail_summary_material_pengadaan_",
       defaults: new { controller = "Dashboard", action = "DetailMaterialPengadaan" });

    //PLANT
    endpoints.MapControllerRoute(
        name: "plant",
        pattern: "plant",
        defaults: new { controller = "Test", action = "Plans" });

    endpoints.MapControllerRoute(
       name: "plant_",
       pattern: "plant_",
       defaults: new { controller = "Test", action = "Get_Plans" });

    endpoints.MapControllerRoute(
       name: "create_plant",
       pattern: "create_plant",
       defaults: new { controller = "Test", action = "Create_Plant" });

    endpoints.MapControllerRoute(
       name: "update_plant",
       pattern: "update_plant",
       defaults: new { controller = "Test", action = "Update_Plant" });

    endpoints.MapControllerRoute(
       name: "delete_plant",
       pattern: "delete_plant",
       defaults: new { controller = "Test", action = "Delete_Plant" });

    //JOBLIST   
    endpoints.MapControllerRoute(
        name: "joblist",
        pattern: "joblist",
        defaults: new { controller = "Joblist", action = "Joblist" });

	endpoints.MapControllerRoute(
	    name: "joblist_",
	    pattern: "joblist_",
	    defaults: new { controller = "Joblist", action = "Get_Joblist" });

    endpoints.MapControllerRoute(
        name: "create_joblist",
        pattern: "create_joblist",
        defaults: new { controller = "Joblist", action = "CreateJoblist" });

    endpoints.MapControllerRoute(
        name: "update_joblist",
        pattern: "update_joblist/{id?}",
        defaults: new { controller = "Joblist", action = "UpdateJoblist" });

    endpoints.MapControllerRoute(
        name: "create_joblist_",
        pattern: "create_joblist_",
        defaults: new { controller = "Joblist", action = "CreateJoblist_" });

    endpoints.MapControllerRoute(
        name: "update_joblist_",
        pattern: "update_joblist_",
        defaults: new { controller = "Joblist", action = "updateJoblist_" });

    endpoints.MapControllerRoute(
        name: "carry_offer_joblist",
        pattern: "carry_offer_joblist",
        defaults: new { controller = "Joblist", action = "CarryOffer" });

    endpoints.MapControllerRoute(
        name: "get_eqtagno",
        pattern: "get_eqtagno",
        defaults: new { controller = "Joblist", action = "getEqtagno" });

    endpoints.MapControllerRoute(
        name: "cek_eqtagno",
        pattern: "cek_eqtagno",
        defaults: new { controller = "Joblist", action = "cekEqtagno" });

    endpoints.MapControllerRoute(
        name: "get_memo",
        pattern: "get_memo",
        defaults: new { controller = "Joblist", action = "getMemo" });

    endpoints.MapControllerRoute(
       name: "get_memo_selected",
       pattern: "get_memo_selected",
       defaults: new { controller = "Joblist", action = "getMemoSelected" });

    endpoints.MapControllerRoute(
        name: "get_notifikasi",
        pattern: "get_notifikasi",
        defaults: new { controller = "Joblist", action = "getNotifikasi" });

    endpoints.MapControllerRoute(
        name: "joblistdetail_",
        pattern: "joblistdetail_",
        defaults: new { controller = "Joblist", action = "JoblistDetail_" });

    endpoints.MapControllerRoute(
        name: "create_joblistdetail_",
        pattern: "create_joblistdetail_",
        defaults: new { controller = "Joblist", action = "createJoblistDetail" });

    endpoints.MapControllerRoute(
        name: "update_joblistdetail_",
        pattern: "update_joblistdetail_",
        defaults: new { controller = "Joblist", action = "updateJoblistDetail" });

    endpoints.MapControllerRoute(
        name: "delete_joblist_detail",
        pattern: "delete_joblist_detail",
        defaults: new { controller = "Joblist", action = "deleteJoblistDetail" });

    endpoints.MapControllerRoute(
        name: "joblist_detail",
        pattern: "joblist_detail",
        defaults: new { controller = "Joblist", action = "Planning" });

    endpoints.MapControllerRoute(
        name: "jobplan_",
        pattern: "jobplan_",
        defaults: new { controller = "Joblist", action = "Get_Joblist_Detail" });

    endpoints.MapControllerRoute(
        name: "update_jobplan",
        pattern: "update_jobplan",
        defaults: new { controller = "Joblist", action = "UpdateJobplan" });

    endpoints.MapControllerRoute(
        name: "get_paket_kontark",
        pattern: "get_paket_kontrak",
        defaults: new { controller = "Joblist", action = "GetPaketKontrak" });

    endpoints.MapControllerRoute(
        name: "get_joblist_wo",
        pattern: "get_joblist_wo",
        defaults: new { controller = "Joblist", action = "GetJoblistWo" });

    endpoints.MapControllerRoute(
        name: "eksekusi_joblist",
        pattern: "eksekusi_joblist",
        defaults: new { controller = "Joblist", action = "EksekusiJoblist" });

    endpoints.MapControllerRoute(
        name: "paket_eksekusi_",
        pattern: "paket_eksekusi_",
        defaults: new { controller = "Joblist", action = "PaketEksekusi" });

    endpoints.MapControllerRoute(
        name: "status_eksekusi",
        pattern: "status_eksekusi",
        defaults: new { controller = "Joblist", action = "StatusEksekusi" });

    endpoints.MapControllerRoute(
        name: "count_eksekusi",
        pattern: "count_eksekusi",
        defaults: new { controller = "Joblist", action = "CountEksekusi" });

    endpoints.MapControllerRoute(
        name: "paket_joblist",
        pattern: "paket_joblist",
        defaults: new { controller = "Joblist", action = "PaketJoblist" });

    endpoints.MapControllerRoute(
        name: "paket_joblist_",
        pattern: "paket_joblist_",
        defaults: new { controller = "Joblist", action = "PaketJoblist_" });

    endpoints.MapControllerRoute(
       name: "detail_paket_joblist",
       pattern: "detail_paket_joblist",
       defaults: new { controller = "Joblist", action = "GetDetailPaket" });

    endpoints.MapControllerRoute(
        name: "create_paket_joblist_",
        pattern: "create_paket_joblist_",
        defaults: new { controller = "Joblist", action = "CreatePaketJoblist" });

    endpoints.MapControllerRoute(
        name: "create_paket_joblist",
        pattern: "create_paket_joblist",
        defaults: new { controller = "Joblist", action = "CreatePaketJoblist_" });

    endpoints.MapControllerRoute(
        name: "edit_paket_joblist_",
        pattern: "edit_paket_joblist_/{id?}",
        defaults: new { controller = "Joblist", action = "UpdatePaketJoblist" });

    endpoints.MapControllerRoute(
        name: "edit_paket_joblist",
        pattern: "edit_paket_joblist",
        defaults: new { controller = "Joblist", action = "UpdatePaketJoblist_" });

    endpoints.MapControllerRoute(
        name: "list_selected",
        pattern: "list_selected",
        defaults: new { controller = "Joblist", action = "ListSelected" });

    endpoints.MapControllerRoute(
        name: "notifikasi",
        pattern: "notifikasi",
        defaults: new { controller = "Joblist", action = "Notifikasi" });

    endpoints.MapControllerRoute(
       name: "notifikasi_",
       pattern: "notifikasi_",
       defaults: new { controller = "Joblist", action = "Get_Notifikasi" });

    endpoints.MapControllerRoute(
      name: "update_rekomendasi",
      pattern: "update_rekomendasi",
      defaults: new { controller = "Joblist", action = "Update_Rekomendasi" });

    endpoints.MapControllerRoute(
      name: "import_notif",
      pattern: "import_notif",
      defaults: new { controller = "Joblist", action = "ImportNotif" });

    endpoints.MapControllerRoute(
       name: "rapat",
       pattern: "rapat",
       defaults: new { controller = "Data", action = "Rapat" });

    endpoints.MapControllerRoute(
       name: "rapat_",
       pattern: "rapat_",
       defaults: new { controller = "Data", action = "Get_Rapat" });

    endpoints.MapControllerRoute(
       name: "create_rapat",
       pattern: "create_rapat",
       defaults: new { controller = "Data", action = "Create_Rapat" });

    endpoints.MapControllerRoute(
       name: "update_rapat",
       pattern: "update_rapat",
       defaults: new { controller = "Data", action = "Update_Rapat" });

    endpoints.MapControllerRoute(
       name: "delete_rapat",
       pattern: "delete_rapat",
       defaults: new { controller = "Data", action = "Deleted_Rapat" });

    endpoints.MapControllerRoute(
      name: "steerco",
      pattern: "steerco",
      defaults: new { controller = "Data", action = "Steerco" });

    endpoints.MapControllerRoute(
       name: "steerco_",
       pattern: "steerco_",
       defaults: new { controller = "Data", action = "Get_Steerco" });

    endpoints.MapControllerRoute(
       name: "create_steerco",
       pattern: "create_steerco",
       defaults: new { controller = "Data", action = "Create_Steerco" });

    endpoints.MapControllerRoute(
       name: "update_steerco",
       pattern: "update_steerco",
       defaults: new { controller = "Data", action = "Update_Steerco" });

    endpoints.MapControllerRoute(
       name: "delete_steerco",
       pattern: "delete_steerco",
       defaults: new { controller = "Data", action = "Deleted_Steerco" });

    endpoints.MapControllerRoute(
       name: "pir",
       pattern: "pir",
       defaults: new { controller = "Data", action = "Pir" });

    endpoints.MapControllerRoute(
       name: "pir_",
       pattern: "pir_",
       defaults: new { controller = "Data", action = "Get_Pir" });

    endpoints.MapControllerRoute(
       name: "create_pir",
       pattern: "create_pir",
       defaults: new { controller = "Data", action = "Create_Pir" });

    endpoints.MapControllerRoute(
       name: "update_pir",
       pattern: "update_pir",
       defaults: new { controller = "Data", action = "Update_Pir" });

    endpoints.MapControllerRoute(
       name: "delete_pir",
       pattern: "delete_pir",
       defaults: new { controller = "Data", action = "Deleted_Pir" });

    endpoints.MapControllerRoute(
       name: "project",
       pattern: "project",
       defaults: new { controller = "Data", action = "Project" });

    endpoints.MapControllerRoute(
       name: "project_",
       pattern: "project_",
       defaults: new { controller = "Data", action = "Get_Project" });

    endpoints.MapControllerRoute(
       name: "create_project",
       pattern: "create_project",
       defaults: new { controller = "Data", action = "Create_Project" });

    endpoints.MapControllerRoute(
       name: "update_project",
       pattern: "update_project",
       defaults: new { controller = "Data", action = "Update_Project" });

    endpoints.MapControllerRoute(
       name: "delete_project",
       pattern: "delete_project",
       defaults: new { controller = "Data", action = "Delete_Project" });

    endpoints.MapControllerRoute(
       name: "equipments",
       pattern: "equipments",
       defaults: new { controller = "Data", action = "Equipments" });

    endpoints.MapControllerRoute(
       name: "equipments_",
       pattern: "equipments_",
       defaults: new { controller = "Data", action = "Get_Equipments" });

    endpoints.MapControllerRoute(
       name: "create_equipment",
       pattern: "create_equipment",
       defaults: new { controller = "Data", action = "Create_Equipments" });

    endpoints.MapControllerRoute(
       name: "update_equipment",
       pattern: "update_equipment",
       defaults: new { controller = "Data", action = "Update_Equipments" });

    endpoints.MapControllerRoute(
       name: "delete_equipment",
       pattern: "delete_equipment",
       defaults: new { controller = "Data", action = "Deleted_Equipments" });

    endpoints.MapControllerRoute(
       name: "getUnitKilang",
       pattern: "getUnitKilang",
       defaults: new { controller = "Data", action = "getUnitKilang" });

    endpoints.MapControllerRoute(
       name: "catalog_profile",
       pattern: "catalog_profile",
       defaults: new { controller = "Data", action = "CatalogProfile" });

    endpoints.MapControllerRoute(
       name: "catalog_profile_",
       pattern: "catalog_profile_",
       defaults: new { controller = "Data", action = "Get_Catalog_Profile" });

    endpoints.MapControllerRoute(
       name: "create_catalog_profile",
       pattern: "create_catalog_profile",
       defaults: new { controller = "Data", action = "Create_Catalog_Profile" });

    endpoints.MapControllerRoute(
       name: "update_catalog_profile",
       pattern: "update_catalog_profile",
       defaults: new { controller = "Data", action = "Update_Catalog_Profile" });

    endpoints.MapControllerRoute(
       name: "delete_catalog_profile",
       pattern: "delete_catalog_profile",
       defaults: new { controller = "Data", action = "Deleted_Catalog_Profile" });

    endpoints.MapControllerRoute(
       name: "request_memo",
       pattern: "request_memo",
       defaults: new { controller = "Data", action = "Memo" });

    endpoints.MapControllerRoute(
       name: "request_memo_",
       pattern: "request_memo_",
       defaults: new { controller = "Data", action = "Get_Memo" });

    endpoints.MapControllerRoute(
       name: "create_request_memo",
       pattern: "create_request_memo",
       defaults: new { controller = "Data", action = "Create_Memo" });

    endpoints.MapControllerRoute(
       name: "update_request_memo",
       pattern: "update_request_memo",
       defaults: new { controller = "Data", action = "Update_Memo" });

    endpoints.MapControllerRoute(
       name: "delete_request_memo",
       pattern: "delete_request_memo",
       defaults: new { controller = "Data", action = "Deleted_Memo" });

    endpoints.MapControllerRoute(
       name: "requestor",
       pattern: "requestor",
       defaults: new { controller = "Data", action = "Requestor" });

    endpoints.MapControllerRoute(
       name: "requestor_",
       pattern: "requestor_",
       defaults: new { controller = "Data", action = "Get_Requestor" });

    endpoints.MapControllerRoute(
       name: "create_requestor",
       pattern: "create_requestor",
       defaults: new { controller = "Data", action = "Create_Requestor" });

    endpoints.MapControllerRoute(
       name: "update_requestor",
       pattern: "update_requestor",
       defaults: new { controller = "Data", action = "Update_Requestor" });

    endpoints.MapControllerRoute(
       name: "delete_requestor",
       pattern: "delete_requestor",
       defaults: new { controller = "Data", action = "Deleted_Requestor" });

    endpoints.MapControllerRoute(
       name: "unit",
       pattern: "unit",
       defaults: new { controller = "Data", action = "Unit" });

    endpoints.MapControllerRoute(
       name: "unit_",
       pattern: "unit_",
       defaults: new { controller = "Data", action = "Get_Unit" });

    endpoints.MapControllerRoute(
       name: "create_unit",
       pattern: "create_unit",
       defaults: new { controller = "Data", action = "Create_Unit" });

    endpoints.MapControllerRoute(
       name: "update_unit",
       pattern: "update_unit",
       defaults: new { controller = "Data", action = "Update_Unit" });

    endpoints.MapControllerRoute(
       name: "delete_unit",
       pattern: "delete_unit",
       defaults: new { controller = "Data", action = "Deleted_Unit" });

    endpoints.MapControllerRoute(
      name: "sow",
      pattern: "sow",
      defaults: new { controller = "Contract", action = "Sow" });

    endpoints.MapControllerRoute(
       name: "sow_",
       pattern: "sow_",
       defaults: new { controller = "Contract", action = "Get_Sow" });

    endpoints.MapControllerRoute(
      name: "get_sow_group",
      pattern: "get_sow_group",
      defaults: new { controller = "Contract", action = "GetSowGroup" });

    endpoints.MapControllerRoute(
       name: "create_sow",
       pattern: "create_sow",
       defaults: new { controller = "Contract", action = "Create_Sow" });
    
    endpoints.MapControllerRoute(
       name: "update_sow",
       pattern: "update_sow",
       defaults: new { controller = "Contract", action = "Update_Sow" });

    endpoints.MapControllerRoute(
       name: "delete_sow",
       pattern: "delete_sow",
       defaults: new { controller = "Contract", action = "Deleted_Sow" });

    endpoints.MapControllerRoute(
     name: "durasi_step",
     pattern: "durasi_step",
     defaults: new { controller = "Contract", action = "DurasiStep" });

    endpoints.MapControllerRoute(
       name: "durasi_step_",
       pattern: "durasi_step_",
       defaults: new { controller = "Contract", action = "Get_DurasiStep" });

    endpoints.MapControllerRoute(
       name: "create_durasi_step",
       pattern: "create_durasi_step",
       defaults: new { controller = "Contract", action = "Create_DurasiStep" });

    endpoints.MapControllerRoute(
        name: "update_durasi_step",
        pattern: "update_durasi_step",
        defaults: new { controller = "Contract", action = "Update_DurasiStep" });

    endpoints.MapControllerRoute(
        name: "delete_durasi_step",
        pattern: "delete_durasi_step",
        defaults: new { controller = "Contract", action = "Delete_DurasiStep" });

    endpoints.MapControllerRoute(
         name: "contracttracking",
         pattern: "contracttracking",
         defaults: new { controller = "Contract", action = "ContractTracking" });

    endpoints.MapControllerRoute(
         name: "contract_",
         pattern: "contract_",
         defaults: new { controller = "Contract", action = "ContractTracking_" });

    endpoints.MapControllerRoute(
         name: "create_contract",
         pattern: "create_contract",
         defaults: new { controller = "Contract", action = "CreateContract" });

    endpoints.MapControllerRoute(
        name: "create_contracttracking",
        pattern: "create_contracttracking",
        defaults: new { controller = "Contract", action = "CreateContract_" });

    endpoints.MapControllerRoute(
        name: "update_contracttracking",
        pattern: "update_contracttracking",
        defaults: new { controller = "Contract", action = "UpdateContract_" });

    endpoints.MapControllerRoute(
         name: "edit_contract/",
         pattern: "edit_contract/{id?}",
         defaults: new { controller = "Contract", action = "UpdateContract" });

    endpoints.MapControllerRoute(
         name: "getKatTender",
         pattern: "getKatTender",
         defaults: new { controller = "Contract", action = "GetKatTender" });

    endpoints.MapControllerRoute(
         name: "get_sow",
         pattern: "get_sow",
         defaults: new { controller = "Contract", action = "GetSow" });

    endpoints.MapControllerRoute(
         name: "related_",
         pattern: "related_",
         defaults: new { controller = "Contract", action = "RelatedJoblist" });

    endpoints.MapControllerRoute(
         name: "delete_wo_jasa",
         pattern: "delete_wo_jasa",
         defaults: new { controller = "Contract", action = "deleteWoJasa" });

    endpoints.MapControllerRoute(
         name: "get_data_target",
         pattern: "get_data_target",
         defaults: new { controller = "Contract", action = "getDataTarget" });

    endpoints.MapControllerRoute(
        name: "user",
        pattern: "user",
        defaults: new { controller = "User", action = "Users" });

    endpoints.MapControllerRoute(
        name: "usermanagemant_",
        pattern: "usermanagemant_",
        defaults: new { controller = "User", action = "Get_User" });

    endpoints.MapControllerRoute(
        name: "create_usermanagement",
        pattern: "create_usermanagement",
        defaults: new { controller = "User", action = "Create_User" });

    endpoints.MapControllerRoute(
        name: "update_usermanagement",
        pattern: "update_usermanagement",
        defaults: new { controller = "User", action = "Update_User" });

    endpoints.MapControllerRoute(
        name: "locked_user",
        pattern: "locked_user",
        defaults: new { controller = "User", action = "Locked_User" });

    endpoints.MapControllerRoute(
        name: "delete_usermanagement",
        pattern: "delete_usermanagement",
        defaults: new { controller = "User", action = "Delete_User" });

    endpoints.MapControllerRoute(
      name: "bom",
      pattern: "bom",
      defaults: new { controller = "Data", action = "Bom" });

    endpoints.MapControllerRoute(
        name: "bom_",
        pattern: "bom_",
        defaults: new { controller = "Data", action = "Get_Boms" });

    endpoints.MapControllerRoute(
        name: "create_bom",
        pattern: "create_bom",
        defaults: new { controller = "Data", action = "Create_Bom" });

    endpoints.MapControllerRoute(
        name: "update_bom",
        pattern: "update_bom",
        defaults: new { controller = "Data", action = "Update_Bom" });

    endpoints.MapControllerRoute(
        name: "delete_bom",
        pattern: "delete_bom",
        defaults: new { controller = "Data", action = "Delete_Bom" });

    //Material
    endpoints.MapControllerRoute(
        name: "material",
        pattern: "material",
        defaults: new { controller = "Material", action = "Material" });

    endpoints.MapControllerRoute(
        name: "material_",
        pattern: "material_",
        defaults: new { controller = "Material", action = "Get_Material" });

    endpoints.MapControllerRoute(
        name: "outstanding_reservasi",
        pattern: "outstanding_reservasi",
        defaults: new { controller = "Material", action = "OutstandingReservasi" });

    endpoints.MapControllerRoute(
        name: "outstanding_reservasi_",
        pattern: "outstanding_reservasi_",
        defaults: new { controller = "Material", action = "OutstandingReservasi_" });

    endpoints.MapControllerRoute(
        name: "penggunaan_material",
        pattern: "penggunaan_material",
        defaults: new { controller = "Material", action = "PenggunaanMaterial" });

    endpoints.MapControllerRoute(
        name: "penggunaan_material_",
        pattern: "penggunaan_material_",
        defaults: new { controller = "Material", action = "PenggunaanMaterial_" });

    endpoints.MapControllerRoute(
        name: "add_prognosa_material",
        pattern: "add_prognosa_material",
        defaults: new { controller = "Material", action = "AddPrognosaMaterial" });
    endpoints.MapControllerRoute(
        name: "edit_pr",
        pattern: "edit_pr",
        defaults: new { controller = "Material", action = "EditPR" });
    endpoints.MapControllerRoute(
        name: "get_work_order",
        pattern: "get_work_order",
        defaults: new { controller = "Material", action = "GetWorkOrder" });
    endpoints.MapControllerRoute(
        name: "get_material_",
        pattern: "get_material_",
        defaults: new { controller = "Material", action = "GetMaterial_" });
    endpoints.MapControllerRoute(
        name: "update_pr",
        pattern: "update_pr",
        defaults: new { controller = "Material", action = "UpdatePR" });
    endpoints.MapControllerRoute(
        name: "list_pr",
        pattern: "list_pr",
        defaults: new { controller = "Material", action = "ListPR" });
    endpoints.MapControllerRoute(
        name: "list_pr_",
        pattern: "list_pr_",
        defaults: new { controller = "Material", action = "ListPR_" });
    endpoints.MapControllerRoute(
        name: "distribusi_pr",
        pattern: "distribusi_pr",
        defaults: new { controller = "Material", action = "DistribusiPR" });
    endpoints.MapControllerRoute(
        name: "distribusi_pr_",
        pattern: "distribusi_pr_",
        defaults: new { controller = "Material", action = "DistribusiPR_" });
    endpoints.MapControllerRoute(
        name: "remove_buyer",
        pattern: "remove_buyer",
        defaults: new { controller = "Material", action = "RemoveBuyer" });
    endpoints.MapControllerRoute(
        name: "doc_pr",
        pattern: "doc_pr",
        defaults: new { controller = "Material", action = "DocPR" });
    endpoints.MapControllerRoute(
        name: "add_status_pengadaan",
        pattern: "add_status_pengadaan",
        defaults: new { controller = "Material", action = "StatusPengadaan" });
    endpoints.MapControllerRoute(
        name: "add_dt",
        pattern: "add_dt",
        defaults: new { controller = "Material", action = "AddDT" });
    endpoints.MapControllerRoute(
        name: "import_material",
        pattern: "import_material",
        defaults: new { controller = "Material", action = "ImportMaterial" });


});

app.Run();
