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

       name: "joblis",
       pattern: "joblist",
       defaults: new { controller = "Joblist", action = "Joblist" });

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
        name: "joblist_detail",
        pattern: "joblist_detail",
        defaults: new { controller = "Joblist", action = "Planning" });

    endpoints.MapControllerRoute(
       name: "joblist_",
       pattern: "joblist_",
       defaults: new { controller = "Joblist", action = "Get_Joblist" });



    endpoints.MapControllerRoute(
       name: "joblist_detail_",
       pattern: "joblist_detail_",
       defaults: new { controller = "Joblist", action = "Get_Joblist_Detail" });

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


});

app.Run();
