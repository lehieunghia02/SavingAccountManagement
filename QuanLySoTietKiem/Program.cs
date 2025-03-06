using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using QuanLySoTietKiem.BackgroundServices;
using QuanLySoTietKiem.BackgroundServices.Interfaces;
using QuanLySoTietKiem.Configurations;
using QuanLySoTietKiem.Constaints;
using QuanLySoTietKiem.Data;
using QuanLySoTietKiem.Entity;
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

#region  Add Services 

builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ISoTietKiemService, SoTietKiemService>();
builder.Services.AddScoped<UserManager<ApplicationUser>>();
builder.Services.AddScoped<IGiaoDichService, GiaoDichService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ILaiSuatDuKienService, LaiSuatDuKienService>();
builder.Services.AddScoped<ITienIchService, TienIchService>();
builder.Services.AddScoped<IAutoUpdateMoneyService, AutoUpdateMoneyServiceImplementation>();
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();
builder.Services.AddScoped<IVNPayService, VNPayService>();
builder.Services.AddScoped<IPayPalService, PayPalService>();

#endregion

#region  Add Host Services
builder.Services.AddHostedService<AutoUpdateMoneyService>();
builder.Services.AddSingleton<IUrlHelperFactory, UrlHelperFactory>();
builder.Services.AddScoped<IUrlHelper>(provider =>
{
    var actionContext = provider.GetRequiredService<IActionContextAccessor>().ActionContext;
    return provider.GetRequiredService<IUrlHelperFactory>().GetUrlHelper(actionContext);
});
builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
#endregion

#region Add Repositories

builder.Services.AddScoped<ILoaiSoTietKiemRepository, LoaiSoTietKiemRepository>();
builder.Services.AddScoped<ILaiSuatDuKienRepository, LaiSuatDuKienRepository>();
builder.Services.AddScoped<ILaiSuatDuKienRepository, LaiSuatDuKienRepository>();
builder.Services.AddScoped<ILoaiSoTietKiemRepository, LoaiSoTietKiemRepository>();
builder.Services.AddScoped<ISoTietKiemRepository, SoTietKiemRepository>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

#endregion

#region Config Settings 

// Update the EmailSettings configuration to map property names correctly
builder.Services.Configure<EmailSettings>(options =>
{
    // First bind the section to get any properties that match directly
    builder.Configuration.GetSection("EmailSettings").Bind(options);

    // Then explicitly map properties with different names
    options.SmtpServer = builder.Configuration["EmailSettings:SmtpServer"] ?? options.SmtpServer;
    options.SmtpPort = int.TryParse(builder.Configuration["EmailSettings:Port"], out int port) ? port : options.SmtpPort;
    options.SmtpUsername = builder.Configuration["EmailSettings:Username"] ?? options.SmtpUsername;
    options.SmtpPassword = builder.Configuration["EmailSettings:Password"] ?? options.SmtpPassword;
    options.FromEmail = builder.Configuration["EmailSettings:SenderEmail"] ?? options.FromEmail;
    options.FromName = builder.Configuration["EmailSettings:SenderName"] ?? options.FromName;
    options.EnableSsl = bool.TryParse(builder.Configuration["EmailSettings:EnableSsl"], out bool enableSsl) ? enableSsl : options.EnableSsl;

    // Log the configuration for debugging
    Console.WriteLine($"Email Configuration:");
    Console.WriteLine($"SmtpServer: {options.SmtpServer}");
    Console.WriteLine($"SmtpPort: {options.SmtpPort}");
    Console.WriteLine($"SmtpUsername: {options.SmtpUsername}");
    Console.WriteLine($"FromEmail: {options.FromEmail}");
    Console.WriteLine($"FromName: {options.FromName}");
    Console.WriteLine($"EnableSsl: {options.EnableSsl}");
});

builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("Cloudinary"));

#endregion

#region Config Dapper

builder.Services.AddSingleton<DapperContext>();

#endregion 

#region Config Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(PolicyConstants.RequireAdmin, policy => policy.RequireRole(RoleConstants.Admin));
    options.AddPolicy(PolicyConstants.RequireUser, policy => policy.RequireRole(RoleConstants.User));
    options.AddPolicy(PolicyConstants.RequireAdminOrUser, policy => policy.RequireRole(RoleConstants.Admin, RoleConstants.User));
});
#endregion


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

// Đăng ký HttpClient cho PayPal
builder.Services.AddHttpClient("PayPal", client =>
{
    // Cấu hình mặc định cho HttpClient
    client.DefaultRequestHeaders.Accept.Clear();
    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseHttpsRedirection();
}
else
{
    // Trong môi trường phát triển, không bắt buộc HTTPS
    // Điều này giúp tránh lỗi SSL khi callback từ PayPal
    app.UseDeveloperExceptionPage();
}

app.UseStatusCodePagesWithReExecute("/Error/{0}");
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();

