using DataAccessLayer;
using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete;
using EntityLayer.Entities;
using KKTCSatiyorum.Mappings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore; 
using System;

var builder = WebApplication.CreateBuilder(args);

// DI
builder.Services.AddScoped<IIlanDal, EfIlanDal>();
builder.Services.AddScoped<IKategoriDal, EfKategoriDal>();
builder.Services.AddScoped<IFavoriDal, EfFavoriDal>();
builder.Services.AddScoped<IBildirimDal, EfBildirimDal>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<IIlanFotografiDal, EfIlanFotografiDal>();
builder.Services.AddScoped<IIlanSikayetiDal, EfIlanSikayetiDal>();
builder.Services.AddScoped<IIlanSorusuDal, EfIlanSorusuDal>();
builder.Services.AddScoped<IIlanAlanDegeriDal, EfIlanAlanDegeriDal>();

builder.Services.AddScoped<IUygulamaKullanicisiDal, EfUygulamaKullanicisiDal>();


//Automapper
builder.Services.AddAutoMapper(
    typeof(AccountProfile).Assembly
);

// Add services to the container.
builder.Services.AddControllersWithViews();

// DbContext konfigürasyonu
builder.Services.AddDbContext<Context>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        x => x.MigrationsAssembly("DataAccessLayer")));

// Identity servislerini ekle
builder.Services.AddIdentity<UygulamaKullanicisi, IdentityRole>(options =>
{
    // Parola gereksinimleri
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
    
    // Kullanýcý ayarlarý
    options.User.RequireUniqueEmail = true;
    
    // Lockout ayarlarý
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
    
    // Sign-in ayarlarý
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
.AddEntityFrameworkStores<Context>()
.AddDefaultTokenProviders();

// Cookie ayarlarý
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
    options.SlidingExpiration = true;
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

// Authorization policy'leri
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("ModeratorOnly", policy => policy.RequireRole("Admin", "Moderator"));
    options.AddPolicy("UserOnly", policy => policy.RequireAuthenticatedUser());
});

var app = builder.Build();

// Uygulama baþlarken Role ve Admin kullanýcýsýný seed et
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await DataAccessLayer.SeedData.InitializeAsync(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Seed iþlemi sýrasýnda bir hata oluþtu.");
    }
}

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

// Authentication middleware (Authorization'dan önce gelmeli!)
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
