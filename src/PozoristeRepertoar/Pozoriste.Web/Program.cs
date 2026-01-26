using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pozoriste.DataAccess.Context;
using Pozoriste.DataAccess.Repositories;
using Pozoriste.Web.Data;

var builder = WebApplication.CreateBuilder(args);

// --- DB (tvoja app baza) ---
builder.Services.AddScoped<IRezervacijaRepository, RezervacijaRepository>();

builder.Services.AddDbContext<PozoristeDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure()));

// --- Identity DB ---
builder.Services.AddDbContext<AppIdentityDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("IdentityConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure()));

// --- Repozitorijumi ---
builder.Services.AddScoped<IPredstavaRepository, PredstavaRepository>();
builder.Services.AddScoped<ITerminRepository, TerminRepository>();
builder.Services.AddScoped<ISalaRepository, SalaRepository>();

// --- Identity ---
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireDigit = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<AppIdentityDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

// MVC + Razor Pages (Identity UI)
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// ⚠️ OVO JE BITNO: NEMA FallbackPolicy!
// Umesto toga, štitiš stranice sa [Authorize] tamo gde treba.
builder.Services.AddAuthorization();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

static async Task ApplyMigrationsAsync(IServiceProvider services)
{
    using var scope = services.CreateScope();

    var appDb = scope.ServiceProvider.GetRequiredService<PozoristeDbContext>();
    await appDb.Database.MigrateAsync();

    var identityDb = scope.ServiceProvider.GetRequiredService<AppIdentityDbContext>();
    await identityDb.Database.MigrateAsync();
}

static async Task SeedAdminAsync(IServiceProvider services)
{
    using var scope = services.CreateScope();

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    const string adminRole = "Admin";
    const string adminEmail = "admin@pozoriste.com";
    const string adminPassword = "Admin123!";

    // role
    if (!await roleManager.RoleExistsAsync(adminRole))
        await roleManager.CreateAsync(new IdentityRole(adminRole));

    // user
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var createRes = await userManager.CreateAsync(adminUser, adminPassword);
        if (!createRes.Succeeded)
            return;
    }

    // add role
    if (!await userManager.IsInRoleAsync(adminUser, adminRole))
        await userManager.AddToRoleAsync(adminUser, adminRole);
}

// migracije + seed
await ApplyMigrationsAsync(app.Services);
await SeedAdminAsync(app.Services);

app.Run();
