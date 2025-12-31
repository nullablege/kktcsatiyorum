using DataAccessLayer;
using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete;
using BusinessLayer.Common;
using EntityLayer.Entities;
using KKTCSatiyorum.Extensions;
using KKTCSatiyorum.Mappings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using BusinessLayer.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddBusinessLayer();

// DI
builder.Services.AddScoped<IIlanDal, EfIlanDal>();
builder.Services.AddScoped<IKategoriDal, EfKategoriDal>();
builder.Services.AddScoped<IKategoriAlaniDal, EfKategoriAlaniDal>();
builder.Services.AddScoped<IKategoriAlaniSecenegiDal, EfKategoriAlaniSecenegiDal>();
builder.Services.AddScoped<IFavoriDal, EfFavoriDal>();
builder.Services.AddScoped<IBildirimDal, EfBildirimDal>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<IIlanFotografiDal, EfIlanFotografiDal>();
builder.Services.AddScoped<IIlanSikayetiDal, EfIlanSikayetiDal>();
builder.Services.AddScoped<IIlanSorusuDal, EfIlanSorusuDal>();
builder.Services.AddScoped<IIlanAlanDegeriDal, EfIlanAlanDegeriDal>();

builder.Services.AddScoped<IUygulamaKullanicisiDal, EfUygulamaKullanicisiDal>();
builder.Services.AddScoped<IDenetimKaydiDal, EfDenetimKaydiDal>();

// Presentation Layer Services
builder.Services.AddScoped<IFileStorage, LocalFileStorage>();

builder.Services.AddSignalR();
builder.Services.AddScoped<BusinessLayer.Common.Abstractions.INotificationPublisher, KKTCSatiyorum.Services.SignalRNotificationPublisher>();


//Automapper
builder.Services.AddAutoMapper(
    typeof(AccountProfile).Assembly
);

// Add services to the container.
builder.Services.AddControllersWithViews();

// DbContext konfigürasyonu
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ICacheService, KKTCSatiyorum.Extensions.MemoryCacheService>();

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
    
    // Kullanıcı ayarları
    options.User.RequireUniqueEmail = true;
    
    // Lockout ayarları
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
    
    // Sign-in ayarları
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
.AddEntityFrameworkStores<Context>()
.AddDefaultTokenProviders();

// Cookie ayarları
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
    options.AddPolicy("AdminOnly", policy => policy.RequireRole(EntityLayer.Constants.RoleNames.Admin));
    options.AddPolicy("ModeratorOnly", policy => policy.RequireRole(EntityLayer.Constants.RoleNames.Admin, EntityLayer.Constants.RoleNames.Moderator));
    options.AddPolicy("UserOnly", policy => policy.RequireAuthenticatedUser());
});

var app = builder.Build();

// Uygulama başlarken Role ve Admin kullanıcısını seed et (Sadece Dev ortamında Migrate/Seed)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await DataAccessLayer.SeedData.InitializeAsync(services, app.Environment.IsDevelopment());
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Seed işlemi sırasında bir hata oluştu.");
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
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<KKTCSatiyorum.Hubs.NotificationsHub>("/hubs/notifications");

app.Run();






