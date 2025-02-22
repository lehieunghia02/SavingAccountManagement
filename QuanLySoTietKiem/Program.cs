using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuanLySoTietKiem.BackgroundServices;
using QuanLySoTietKiem.BackgroundServices.Interfaces;
using QuanLySoTietKiem.Configurations;
using QuanLySoTietKiem.Data;
using QuanLySoTietKiem.Models;
using QuanLySoTietKiem.Repositories.Implementations;
using QuanLySoTietKiem.Repositories.Interfaces;
using QuanLySoTietKiem.Services;
using QuanLySoTietKiem.Services.Implementations;
using QuanLySoTietKiem.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DeployConnection")));


builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();



//add services
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ISoTietKiemService, SoTietKiemService>();
builder.Services.AddScoped<UserManager<ApplicationUser>>();
builder.Services.AddScoped<IGiaoDichService, GiaoDichService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ILaiSuatDuKienService, LaiSuatDuKienService>();
builder.Services.AddScoped<ILoaiSoTietKiemRepository, LoaiSoTietKiemRepository>();
builder.Services.AddScoped<ILaiSuatDuKienRepository, LaiSuatDuKienRepository>();
builder.Services.AddHostedService<AutoUpdateMoneyService>();
builder.Services.AddScoped<IAutoUpdateMoneyService, AutoUpdateMoneyServiceImplementation>();
builder.Services.AddScoped<ITienIchService, TienIchService>();
builder.Services.AddScoped<ILaiSuatDuKienRepository, LaiSuatDuKienRepository>();
builder.Services.AddScoped<ILoaiSoTietKiemRepository, LoaiSoTietKiemRepository>();

// Add Configuration settings 
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

//Config Logging 
builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddConsole();
    loggingBuilder.AddDebug();
});
//seeder role 

// using (var scope = builder.Services.BuildServiceProvider().CreateScope())
// {
//     var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
//     var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

//     await RoleSeeder.SeedRolesAsync(roleManager);

// }

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/Error/{0}");
app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
