using Microsoft.EntityFrameworkCore;
using Registration_Login_ASP_Dot_Net_MVC.Data;
using Registration_Login_ASP_Dot_Net_MVC.Interfaces.AccountInterfaces;
using Registration_Login_ASP_Dot_Net_MVC.Services.AccountService;
using FluentValidation.AspNetCore;
using FluentValidation;
using Serilog;
using Registration_Login_ASP_Dot_Net_MVC.Interfaces.EmailInterface;
using Registration_Login_ASP_Dot_Net_MVC.Interfaces.OtpInterface;
using Registration_Login_ASP_Dot_Net_MVC.Services.EmailService;
using Registration_Login_ASP_Dot_Net_MVC.Services.OtpService;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add DbContext before calling Build()
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AccountDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add the AccountService and IAccountInterface to the services container
builder.Services.AddScoped<IAccountInterface, AccountService>();
builder.Services.AddScoped<IOtpInterface, OtpService>();
builder.Services.AddScoped<IEmailInterface, EmailService>();

// Add FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignOutScheme = Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie();

// Enable session state
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

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
