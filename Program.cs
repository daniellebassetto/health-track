using HealthTrack.Core.Interfaces.Repositories;
using HealthTrack.Core.Interfaces.Services;
using HealthTrack.Core.Models;
using HealthTrack.Core.Models.Entities;
using HealthTrack.Core.Services;
using HealthTrack.Core.Utils;
using HealthTrack.Infrastructure.Data;
using HealthTrack.Infrastructure.Repositories;
using HealthTrack.Middleware;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<HealthTrackDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
    new MySqlServerVersion(new Version(8, 0, 21))));

builder.Services.AddDefaultIdentity<User>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<HealthTrackDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.LoginPath = "/Auth/Login";
    options.LogoutPath = "/Auth/Logout";
    options.AccessDeniedPath = "/Auth/AccessDenied";
});

builder.Services.AddRazorPages();

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { new CultureInfo("pt-BR") };
    options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("pt-BR");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

builder.Services.Configure<GeminiApiOptions>(builder.Configuration.GetSection(GeminiApiOptions.SectionName));
builder.Services.AddScoped<AiExamExtractor>();

builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IExamRepository, ExamRepository>();
builder.Services.AddScoped<IExamParameterRepository, ExamParameterRepository>();

builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IExamService, ExamService>();

builder.Services.AddScoped<AuthService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseMiddleware<ErrorHandlingMiddleware>();
    app.UseHsts();
}
else
    app.UseDeveloperExceptionPage();

app.UseHttpsRedirection();
app.UseRequestLocalization();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages();

app.Run();
