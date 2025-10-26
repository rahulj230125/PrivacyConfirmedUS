# Quick Start Guide - PrivacyConfirmed API

## ?? Start the API in 3 Steps

### Step 1: Run the Database Setup Script
Choose your database provider:

**SQL Server:**
```bash
# Run the script in SQL Server Management Studio or using sqlcmd
sqlcmd -S localhost -i DatabaseScripts\SqlServer_Setup.sql
```

**PostgreSQL:**
```bash
# Run the script using psql
psql -U postgres -f DatabaseScripts\PostgreSQL_Setup.sql
```

### Step 2: Configure Connection String
Open `PrivacyConfirmedAPI/appsettings.json` and update:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=PrivacyConfirmedDB;Trusted_Connection=True;TrustServerCertificate=True;",
    "PostgreSQLConnection": "Host=YOUR_HOST;Database=privacyconfirmeddb;Username=YOUR_USER;Password=YOUR_PASSWORD;"
  },
  "DatabaseProvider": "SqlServer"  // or "PostgreSQL"
}
```

### Step 3: Run the API
**Visual Studio:**
- Right-click `PrivacyConfirmedAPI` project
- Select "Set as Startup Project"
- Press F5

**Command Line:**
```bash
cd PrivacyConfirmedAPI
dotnet run
```

## ? Verify It's Working

1. Open browser to: `https://localhost:7002`
2. You should see the Swagger UI
3. Try the Health Check endpoint: `GET /api/contactus/health`

## ?? Test the Contact Form

### Using Swagger UI:
1. Click on `POST /api/contactus`
2. Click "Try it out"
3. Enter sample data:
```json
{
  "name": "John Doe",
  "company": "Tech Solutions Inc",
  "mobileNumber": "9876543210",
  "email": "john.doe@example.com"
}
```
4. Click "Execute"
5. You should see a 200 response with success message

### Using PowerShell:
```powershell
$body = @{
    name = "John Doe"
    company = "Tech Solutions Inc"
    mobileNumber = "9876543210"
    email = "john.doe@example.com"
} | ConvertTo-Json

Invoke-RestMethod -Uri "https://localhost:7002/api/contactus" `
                  -Method Post `
                  -Body $body `
                  -ContentType "application/json"
```

## ?? Connect from MVC UI

Add to your MVC project's `Program.cs`:

```csharp
// Add HttpClient
builder.Services.AddHttpClient("PrivacyConfirmedAPI", client =>
{
    client.BaseAddress = new Uri("https://localhost:7002");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});
```

Use in your controller:

```csharp
public class HomeController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public HomeController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpPost]
    public async Task<IActionResult> ContactUs(ContactUsModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var client = _httpClientFactory.CreateClient("PrivacyConfirmedAPI");
        
        try
        {
            var response = await client.PostAsJsonAsync("/api/contactus", model);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse>();
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction("ContactUsSuccess");
            }
            else
            {
                var errorResult = await response.Content.ReadFromJsonAsync<ApiResponse>();
                foreach (var error in errorResult.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Unable to connect to API. Please try again later.");
        }

        return View(model);
    }
}
```

## ?? Troubleshooting

### Port Already in Use
If ports 7002/5002 are already in use:
1. Open `PrivacyConfirmedAPI/Properties/launchSettings.json`
2. Change the ports in `applicationUrl`
3. Update CORS origins in `Program.cs`

### Database Connection Failed
1. Verify SQL Server/PostgreSQL is running
2. Check connection string in `appsettings.json`
3. Ensure database exists (run setup scripts)
4. Test connection using SQL Server Management Studio or pgAdmin

### CORS Error from MVC UI
1. Verify MVC UI URL is in CORS origins (in API's `Program.cs`)
2. Ensure both projects are running
3. Check browser console for specific CORS error

## ?? Full Documentation

For complete documentation, see:
- `README.md` - Comprehensive API documentation
- `IMPLEMENTATION_SUMMARY.md` - Implementation details
- `ContactUsAPI.http` - Example HTTP requests

## ?? You're All Set!

Your API is now ready to handle Contact Us form submissions with:
- ? Full CRUD operations
- ? Comprehensive validation
- ? Structured error handling
- ? Interactive Swagger documentation
- ? CORS enabled for MVC UI integration

Happy coding! ??
