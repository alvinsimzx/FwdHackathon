using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FwdHackathon.Areas.Identity.Data;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("AppDbContextConnection") ?? throw new InvalidOperationException("Connection string 'AppDbContextConnection' not found.");

var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

// Add MySQL DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
{
  string connectionString;
  var serverVersion = new MySqlServerVersion(new Version(8, 0, 28));

  if (env == "Production")
  {
    // If the production environment, use connection string in heroku config vars
    var connUser = Environment.GetEnvironmentVariable("Username");
    var connPass = Environment.GetEnvironmentVariable("Password");
    var connHost = Environment.GetEnvironmentVariable("Host");
    var connDb = Environment.GetEnvironmentVariable("Database");

    connectionString = $"server={connHost};Uid={connUser};Pwd={connPass};Database={connDb}";
  }
  else
  {
    // If development environment, use connection string in appsettings config
    connectionString = builder.Configuration.GetConnectionString("ApplicationDbContextConnection") ?? throw new InvalidOperationException("Connection string 'ApplicationDbContextConnection' not found.");
  }

  options.UseMySql(connectionString, serverVersion);
});

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<AppDbContext>();

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
app.UseAuthentication();;

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
