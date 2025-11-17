using PrivacyConfirmedBAL.Interfaces;
using PrivacyConfirmedBAL.Services;
using PrivacyConfirmedDAL.Interfaces;
using PrivacyConfirmedDAL.Repositories;
using Serilog;

// Configure Serilog early in the application startup
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting PrivacyConfirmedAPI application");

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Configure Serilog ENTIRELY from appsettings.json
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

    // Add services to the container.
    builder.Services.AddControllers();

    // Register Dependency Injection for DAL and BAL
    builder.Services.AddScoped<IContactUsRepository, ContactUsRepository>();
    builder.Services.AddScoped<IContactUsService, ContactUsService>();
    builder.Services.AddScoped<IResourceFileRepository, ResourceFileRepository>();
    builder.Services.AddScoped<IResourceFileService, ResourceFileService>();

    // Configure CORS to allow requests from the MVC UI project
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowMvcUI", policy =>
        {
            policy.WithOrigins("https://localhost:7001", "http://localhost:5001")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
    });

    // Add API Explorer and Swagger for API documentation
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0.0";
        c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "PrivacyConfirmed API",
            Version = $"v{version}",
            Description = "REST API for PrivacyConfirmed application - Contact Us & Resource Center features",
            Contact = new Microsoft.OpenApi.Models.OpenApiContact
            {
                Name = "PrivacyConfirmed Team"
            }
        });
    });

    var app = builder.Build();

    // Add Serilog request logging
    app.UseSerilogRequestLogging();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "PrivacyConfirmed API v1");
            c.RoutePrefix = string.Empty;
        });
    }

    app.UseHttpsRedirection();

    // Enable CORS
    app.UseCors("AllowMvcUI");

    app.UseAuthorization();

    app.MapControllers();

    Log.Information("PrivacyConfirmedAPI application started successfully");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "API application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
