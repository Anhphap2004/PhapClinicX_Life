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

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthorization();

app.UseAuthorization();
app.UseSession();
app.UseMiddleware<RoleMiddleware>();

app.MapStaticAssets();


app.MapControllerRoute(
    name: "areas",

app.MapControllerRoute(
    name: "default",

app.Run();