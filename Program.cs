using dotenv.net;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using ProjIS.Data;
using ProjIS.Models;

DotEnv.Load();

var builder = WebApplication.CreateBuilder(args);

var dbHost = Environment.GetEnvironmentVariable("SUPABASE_DB_HOST");
var dbPort = Environment.GetEnvironmentVariable("SUPABASE_DB_PORT");
var dbUser = Environment.GetEnvironmentVariable("SUPABASE_DB_USER");
var dbPassword = Environment.GetEnvironmentVariable("SUPABASE_DB_PASSWORD");

var connectionString =
    $"Host={dbHost};Port={dbPort};Database=postgres;Username={dbUser};" +
    $"Password={dbPassword};SSL Mode=Require;Trust Server Certificate=true";

var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
dataSourceBuilder.MapEnum<FlightType>("flight_type");
dataSourceBuilder.MapEnum<FlightClass>("flight_class");
dataSourceBuilder.MapEnum<PaymentMethod>("payment_method");
dataSourceBuilder.MapEnum<PaymentStatus>("payment_status");
var dataSource = dataSourceBuilder.Build();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(dataSource, npg =>
    {
        npg.MapEnum<FlightType>("flight_type");
        npg.MapEnum<FlightClass>("flight_class");
        npg.MapEnum<PaymentMethod>("payment_method");
        npg.MapEnum<PaymentStatus>("payment_status");
    }));

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/auth/airline";
        options.AccessDeniedPath = "/auth/airline";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Cookie.Name = "ProjIS.Auth";
    });

builder.Services.AddAuthorization();
builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();  
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();