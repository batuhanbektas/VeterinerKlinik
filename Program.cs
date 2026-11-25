using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VetClinic.Data;
using VetClinic.Models.Entities;

var builder = WebApplication.CreateBuilder(args);

// bağlantı dizesi user secrets'ta, appsettings'e koymadım
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Bağlantı dizesi bulunamadı, user-secrets ile ekle.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// identity: kullanıcı + rol yönetimi
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        // admin şifresi "sau" olacağı için parola kurallarını gevşettim
        options.Password.RequiredLength = 3;
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredUniqueChars = 1;

        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedAccount = false; // e-posta onayı yok
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddErrorDescriber<TurkishIdentityErrorDescriber>();

// giriş yapmamış biri yetkili sayfaya girerse login'e, yetkisi yoksa erişim engellendi sayfasına
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

// açılışta migration + roller + admin + örnek veri
using (var scope = app.Services.CreateScope())
{
    await DbSeeder.SeedAsync(scope.ServiceProvider);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// sıra önemli: önce kimlik (kim bu?), sonra yetki (buna izni var mı?)
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
