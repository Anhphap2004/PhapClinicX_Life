using Microsoft.EntityFrameworkCore;
using PhapClinicX.Middleware;
using PhapClinicX.Models;
using PhapClinicX.Services;
using PhapClinicX.Services.Vnpay;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ClinicManagementContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddHttpClient();

builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<IVnPayService, VnPayService>();
builder.Services.AddScoped<IChatService, ChatService>();


builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.WebHost.UseUrls("http://localhost:44340", "http://0.0.0.0:44340");

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthorization();

app.UseSession();
app.UseMiddleware<RoleMiddleware>();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
).WithStaticAssets();

app.Run();
