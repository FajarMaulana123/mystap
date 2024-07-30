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

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
		name: "awal",
		pattern: "awal",
		defaults: new { controller = "Awal", action = "Awal" });

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

    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Awal}/{action=Awal}/{id?}");

    endpoints.MapControllerRoute(

       name: "joblist",
       pattern: "joblist",
       defaults: new { controller = "Joblist", action = "Joblist" });

    endpoints.MapControllerRoute(
       name: "joblist_detail",
       pattern: "joblist_detail",
       defaults: new { controller = "Joblist", action = "Planning" });

    endpoints.MapControllerRoute(
      name: "joblist_detail_",
      pattern: "joblist_detail_",
      defaults: new { controller = "Joblist", action = "Get_Joblist_Detail" });

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
       defaults: new { controller = "Data", action = "Delete_Rapat" });

    endpoints.MapControllerRoute(
       name: "project",
       pattern: "project",
       defaults: new { controller = "Data", action = "Project" });

    endpoints.MapControllerRoute(
       name: "project_",
       pattern: "project_",
       defaults: new { controller = "Data", action = "Project" });

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
       name: "create_equipments",
       pattern: "create_equipments",
       defaults: new { controller = "Data", action = "Create_Equipments" });

    endpoints.MapControllerRoute(
       name: "update_equipments",
       pattern: "update_equipments",
       defaults: new { controller = "Data", action = "Update_Equipments" });

    endpoints.MapControllerRoute(
       name: "delete_equipments",
       pattern: "delete_equipments",
       defaults: new { controller = "Data", action = "Delete_Equipments" });

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
       defaults: new { controller = "Data", action = "Delete_Catalog_Profile" });

    endpoints.MapControllerRoute(
       name: "request_memo",
       pattern: "request_memo",
       defaults: new { controller = "Data", action = "Memo" });

    endpoints.MapControllerRoute(
       name: "request_memo_",
       pattern: "request_memo_",
       defaults: new { controller = "Data", action = "Get_Memo" });

    endpoints.MapControllerRoute(
       name: "create_memo",
       pattern: "create_memo",
       defaults: new { controller = "Data", action = "Create_Memo" });

    endpoints.MapControllerRoute(
       name: "update_memo",
       pattern: "update_memo",
       defaults: new { controller = "Data", action = "Update_Memo" });

    endpoints.MapControllerRoute(
       name: "delete_memo",
       pattern: "delete_memo",
       defaults: new { controller = "Data", action = "Delete_Memo" });

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
       defaults: new { controller = "Data", action = "Delete_Requestor" });

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
       defaults: new { controller = "Data", action = "Delete_Unit" });

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
      defaults: new { controller = "Contract", action = "Get_SowGroup" });

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
       defaults: new { controller = "Contract", action = "Delete_Sow" });


});

app.Run();
