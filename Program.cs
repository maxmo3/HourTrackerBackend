using System.Text.Json;
using System.Text.Json.Serialization;
using HourTrackerBackend.Helpers;
using HourTrackerBackend.Modals;
using HourTrackerBackend.Modals.Database;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.WriteIndented = true;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<TrackerContext>();

builder.Services
    .AddIdentity<User, IdentityRole>(o =>
    {
        o.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        o.Lockout.MaxFailedAccessAttempts = 5;
    })
    .AddEntityFrameworkStores<TrackerContext>()
    .AddDefaultTokenProviders();

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(
        CookieAuthenticationDefaults.AuthenticationScheme,
        o =>
        {
            o.Cookie.SameSite = SameSiteMode.None;
            o.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            o.ExpireTimeSpan = TimeSpan.FromDays(7);
            o.SlidingExpiration = true;
        }
    );


builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromDays(7);
});

builder.Services.Configure<IdentityOptions>(o =>
{
    o.Password.RequireNonAlphanumeric = false;
    o.Password.RequireDigit = true;
    o.Password.RequireUppercase = true;
    o.Password.RequireLowercase = true;
    o.Password.RequiredLength = 6;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SameSite = SameSiteMode.None;
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = 401;
        return Task.CompletedTask;
    };
});
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<LinkHelper>();
builder.Services.AddScoped<CommonHelper>();
builder.Services.AddScoped<MechanicHelper>();
builder.Services.AddScoped<ProjectHelper>();
builder.Services.AddScoped<TodoHelper>();
builder.Services.AddScoped<GeneralHelper>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.Requirements.Add(new AdminRequirement()));
});
builder.Services.AddScoped<IAuthorizationHandler, AdminAuthorizationHandler>();


var app = builder.Build();

// Configure path base from environment variable (for deployment behind ingress with prefix)
var pathBase = Environment.GetEnvironmentVariable("PATH_BASE") ?? string.Empty;
if (!string.IsNullOrEmpty(pathBase))
{
    app.UsePathBase(pathBase);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        var swaggerPath = string.IsNullOrEmpty(pathBase) ? "/swagger/v1/swagger.json" : $"{pathBase}/swagger/v1/swagger.json";
        var stylePath = string.IsNullOrEmpty(pathBase) ? "/swagger-ui/SwaggerDark.css" : $"{pathBase}/swagger-ui/SwaggerDark.css";
        c.SwaggerEndpoint(swaggerPath, "MyAPI");
        c.InjectStylesheet(stylePath);
    });
}

app.UseCors(
    b =>
        b.SetIsOriginAllowed(
                origin =>
                    new Uri(origin).Host == "localhost" || true
            )
            .AllowCredentials()
            .AllowAnyMethod()
            .AllowAnyHeader()
);

// HTTPS redirection handled by ingress
// app.UseHttpsRedirection();

app.UseCookiePolicy(new CookiePolicyOptions() {
    MinimumSameSitePolicy = SameSiteMode.None,
    Secure = CookieSecurePolicy.Always
});

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TrackerContext>();

    try
    {
        dbContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        // Log but don't crash — the raw SQL below handles the schema defensively.
        Console.WriteLine($"[STARTUP] Migrate() failed (non-fatal): {ex.Message}");
    }

    // Idempotent column guard — runs even if Migrate() failed or was already applied.
    // ADD COLUMN IF NOT EXISTS is a no-op when the column already exists.
    dbContext.Database.ExecuteSqlRaw("""
        ALTER TABLE "Projects" ADD COLUMN IF NOT EXISTS "MeerwerkSeconds" integer NOT NULL DEFAULT 0;
        ALTER TABLE "Projects" ADD COLUMN IF NOT EXISTS "DhzSeconds" integer NOT NULL DEFAULT 0;
        ALTER TABLE "AspNetUsers" ADD COLUMN IF NOT EXISTS "IsAdmin" boolean NOT NULL DEFAULT false;
        ALTER TABLE "Projects" ADD COLUMN IF NOT EXISTS "CreatedByUserName" text NULL;
        """);

    // Seed admin users — these usernames always get IsAdmin=true on startup.
    var adminUsernames = new[] { "timo" };
    foreach (var adminName in adminUsernames)
    {
        var adminUser = dbContext.Users.FirstOrDefault(u => u.UserName == adminName);
        if (adminUser != null && !adminUser.IsAdmin)
        {
            adminUser.IsAdmin = true;
            dbContext.SaveChanges();
        }
    }
}

app.UseAuthorization();

app.UseStaticFiles();

app.MapControllers();

app.Run();
