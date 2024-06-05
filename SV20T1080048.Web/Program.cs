using SV20T1080048.Web;
using SV20T1080048.Web.AddCodes;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Session;
using Microsoft.Extensions.Caching.Distributed;

var builder = WebApplication.CreateBuilder(args);

// Cấu hình Redis Cache
var redisConnectionString = builder.Configuration.GetSection("RedisCacheSettings:ConnectionString").Value;
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnectionString;
});

// Cấu hình các service khác
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews()
    .AddMvcOptions(option =>
    {
        option.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
    });

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "UserScheme";
})
.AddCookie("UserScheme", options =>
{
    options.Cookie.Name = "UserAuthCookie";
    options.LoginPath = "/UserAccount/Login";
    options.AccessDeniedPath = "/UserAccount/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
})
.AddCookie("AdminScheme", options =>
{
    options.Cookie.Name = "AdminAuthCookie";
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.UseEndpoints(endpoints =>
{
    endpoints.MapAreaControllerRoute(
        name: "areaAdmin",
        areaName: "Admin",
        pattern: "admin/{controller=Dashboard}/{action=Index}/{id?}"
    );
    endpoints.MapControllerRoute(
      name: "areas",
      pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    );
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Khởi tạo cấu hình cho ApplicationContext với Redis Cache
ApplicationContext.Configure(
    app.Services.GetRequiredService<IHttpContextAccessor>(),
    app.Services.GetService<IWebHostEnvironment>(),
    app.Services.GetRequiredService<IDistributedCache>()
);

app.Run();