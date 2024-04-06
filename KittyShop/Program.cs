using KittyShop.Data.DBContext;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using KittyShop.Services.Utility;
using KittyShop.Services;
using KittyShop.Interfaces.IServices;
using KittyShop.Interfaces.IRepositories;
using KittyShop.Repositories;
using Serilog;
using Microsoft.AspNetCore.Authentication.Cookies;


var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) => 
configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        
        options.ExpireTimeSpan = TimeSpan.FromDays(1);
        options.SlidingExpiration = true; 
        options.LoginPath = new PathString("/Home/Login");
        options.AccessDeniedPath = new PathString("/Home/Login");
    });


builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<KittyShopContext>(dbContextOptions => dbContextOptions.UseSqlServer(
	builder.Configuration["ConnectionStrings:KittyShopDBConnectionString"], optionAction =>
     optionAction.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null)));

builder.Services.AddDataProtection()
       .UseCryptographicAlgorithms(new AuthenticatedEncryptorConfiguration()
       {
           EncryptionAlgorithm = EncryptionAlgorithm.AES_256_GCM,
           ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
       });

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddSingleton<CipherService>();
builder.Services.AddSingleton<ImageSevice>();

builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IHomeService, HomeService>();
builder.Services.AddScoped<IHomeRepository, HomeRepository>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Login}");

app.Run();
