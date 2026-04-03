using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using PrivacyConfirmedDAL.Interfaces;
using PrivacyConfirmedDAL.Repositories;
using PrivacyConfirmedBAL.Interfaces;
using PrivacyConfirmedBAL.Services;
using Serilog;

// Configure Serilog early in the application startup
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting PrivacyConfirmed web application");

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Configure Serilog from appsettings.json
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

    // Add services to the container.
    builder.Services.AddRazorPages();
    builder.Services.AddControllersWithViews();

    // Register HttpClient for API calls
    builder.Services.AddHttpClient();

    // Register Dependency Injection for DAL and BAL
    builder.Services.AddScoped<IContactUsRepository, ContactUsRepository>();
    builder.Services.AddScoped<IContactUsService, ContactUsService>();
    builder.Services.AddScoped<IResourceFileRepository, ResourceFileRepository>();
    builder.Services.AddScoped<IResourceFileService, ResourceFileService>();

    // Build PostgreSQL connection string from environment variables or appsettings.json
    string pgHost = Environment.GetEnvironmentVariable("pc_host");
    string pgDatabase = Environment.GetEnvironmentVariable("pc_database");
    string pgUsername = Environment.GetEnvironmentVariable("pc_username");
    string pgPassword = Environment.GetEnvironmentVariable("pc_password");

    string pgConnectionString;
    if (!string.IsNullOrWhiteSpace(pgHost) &&
        !string.IsNullOrWhiteSpace(pgDatabase) &&
        !string.IsNullOrWhiteSpace(pgUsername) &&
        !string.IsNullOrWhiteSpace(pgPassword))
    {
        pgConnectionString = $"Host={pgHost};Database={pgDatabase};Username={pgUsername};Password={pgPassword};";
    }
    else
    {
        pgConnectionString = builder.Configuration.GetConnectionString("PostgreSQLConnection");
    }

    // Register Health Checks: use built connection string
    builder.Services.AddHealthChecks()
        .AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy())
        .AddNpgSql(pgConnectionString, name: "postgresql");

    var app = builder.Build();

    // Add Serilog request logging
    app.UseSerilogRequestLogging();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios.
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthorization();

    // Map health check endpoint
    app.MapHealthChecks("/health", new HealthCheckOptions
    {
        ResponseWriter = async (context, report) =>
        {
            context.Response.ContentType = "application/json";
            var result = System.Text.Json.JsonSerializer.Serialize(new
            {
                status = report.Status.ToString(),
                checks = report.Entries.Select(e => new { name = e.Key, status = e.Value.Status.ToString(), exception = e.Value.Exception?.Message, duration = e.Value.Duration.ToString() })
            });
            await context.Response.WriteAsync(result);
        }
    });

    app.MapRazorPages();
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    Log.Information("PrivacyConfirmed application started successfully");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
