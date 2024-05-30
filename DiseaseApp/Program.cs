using DiseaseApp.Concrete.EfCore;
using DiseaseApp.Data.Abstract;
using DiseaseApp.Data.Concrete.EfCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<DiseaseContext>(options =>
{
    var config = builder.Configuration;
    var connectionString = config.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connectionString);
});

// requests for IUserRepository should be handled by EfUserRepository
builder.Services.AddScoped<IPostRepository, EfPostRepository>();
builder.Services.AddScoped<ITagRepository, EfTagRepository>();
builder.Services.AddScoped<ICommentRepository, EfCommentRepository>();
builder.Services.AddScoped<IUserRepository, EfUserRepository>();
builder.Services.AddScoped<IAdminRepository, EfAdminRepository>();
//Cookie based authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.LoginPath = "/Users/Login";
    options.AccessDeniedPath = "/Users/AccessDenied";
});


var app = builder.Build();

//Activate wwwroot folder
app.UseStaticFiles();

//Authentication and Authorization middleware
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
//Test data
SeedData.FillTestData(app);

// posts/url/seo-friendly-url

//Controller routes
app.MapControllerRoute(
    name: "post_details",
    pattern: "/posts/details/{url}",
    defaults: new { controller = "Posts", action = "Details" });

app.MapControllerRoute(
    name: "post_by_tag",
    pattern: "/posts/tag/{tag}",
    defaults: new { controller = "Posts", action = "Index" });


app.MapControllerRoute(
    name: "user_profile",
    pattern: "/profile/{username}",
    defaults: new { controller = "Users", action = "Profile" });

//Default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Posts}/{action=Index}/{id?}");

//Admin management area
app.MapAreaControllerRoute(
    name: "admin",
    areaName: "Admin",
    pattern: "/Admin/{controller=Admin}/{action=UserManager}");

app.MapAreaControllerRoute(
    name: "admin",
    areaName: "Admin",
    pattern: "/Admin/{controller=Admin}/{action=PostManager}");

app.MapAreaControllerRoute(
    name: "admin",
    areaName: "Admin",
    pattern: "/Admin/{controller=Admin}/{action=CommentManager}");

app.MapAreaControllerRoute(
    name: "admin_user_edit",
    areaName: "Admin",
    pattern: "Admin/{controller=Admin}/{action=UserEdit}/{UserId?}");

app.Run();

