using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using EbbinghausFlashcardApp.Data;
using EbbinghausFlashcardApp.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.SignalR;
using EbbinghausFlashcardApp.Hubs;
using EbbinghausFlashcardApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages();

// add db context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// add hosted service
builder.Services.AddHostedService<ReviewNotificationService>();

// disable email confirmation and add identity procedure
// I don't want to have email confimation for this time, maybe added it later. But the database still accept email field
builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();

/* Acknowledgement: This code is based on the following part: "Cookie settings" from the following reference.
 * @reference: https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity-configuration?view=aspnetcore-8.0 
 */
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.SlidingExpiration = true;
    options.Cookie.Name = ".EbbinghausFlashcardApp.Cookie";
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

// real-time update
builder.Services.AddSignalR();

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

app.Use(async (context, next) =>
{
    var path = context.Request.Path;

    // List of paths that should be accessible without authentication
    var allowedPaths = new[]
    {
        "/Identity/Account/Login",
        "/Identity/Account/Logout",
        "/Identity/Account/Register",
        "/Identity/Account",
        "/Home/Privacy",
        "/lib",
        "/css",
        "/js",
        "/favicon.ico",
        "/notificationHub"
    };

    if (!context.User.Identity.IsAuthenticated &&
        !allowedPaths.Any(p => path.StartsWithSegments(p)))
    {
        context.Response.Redirect("/Identity/Account/Login");
        return;
    }
    await next();
});
app.MapRazorPages();
app.MapControllers();

app.MapHub<NotificationHub>("/notificationHub");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
//app.MapHub<FlashcardHub>("/flashcardHub");
app.Run();