using PrivacyConfirmedDAL.Interfaces;
using PrivacyConfirmedDAL.Repositories;
using PrivacyConfirmedBAL.Interfaces;
using PrivacyConfirmedBAL.Services;

var builder = WebApplication.CreateBuilder(args);

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

var app = builder.Build();

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

app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
