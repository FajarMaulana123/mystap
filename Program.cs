using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;
using mystap.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddControllersWithViews();


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
       name: "project",
       pattern: "project",
       defaults: new { controller = "Data", action = "Project" });

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
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.Run();
