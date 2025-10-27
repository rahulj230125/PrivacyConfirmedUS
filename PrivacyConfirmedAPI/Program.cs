using PrivacyConfirmedBAL.Interfaces;
using PrivacyConfirmedBAL.Services;
using PrivacyConfirmedDAL.Interfaces;
using PrivacyConfirmedDAL.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Register Dependency Injection for DAL and BAL
builder.Services.AddScoped<IContactUsRepository, ContactUsRepository>();
builder.Services.AddScoped<IContactUsService, ContactUsService>();

// Configure CORS to allow requests from the MVC UI project
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMvcUI", policy =>
    {
        policy.WithOrigins("https://localhost:7001", "http://localhost:5001") // Adjust ports as needed
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Add API Explorer and Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "PrivacyConfirmed API",
        Version = "v1",
        Description = "REST API for PrivacyConfirmed application - Contact Us feature",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "PrivacyConfirmed Team"
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PrivacyConfirmed API v1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
    });
}

app.UseHttpsRedirection();

// Enable CORS
app.UseCors("AllowMvcUI");

app.UseAuthorization();

app.MapControllers();

app.Run();
