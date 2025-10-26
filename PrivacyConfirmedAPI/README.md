# PrivacyConfirmed API

A REST API project that exposes endpoints for the PrivacyConfirmed application. This API provides backend services for the Contact Us feature.

## ?? Features

- **RESTful API** for Contact Us operations
- **CORS enabled** to allow requests from the MVC UI project
- **Swagger/OpenAPI** documentation for easy testing and integration
- **Structured responses** with consistent error handling
- **Dependency Injection** for BAL and DAL layers
- **Comprehensive logging** for monitoring and debugging

## ?? Prerequisites

- .NET 8.0 SDK or later
- SQL Server or PostgreSQL database
- PrivacyConfirmedModel, PrivacyConfirmedBAL, and PrivacyConfirmedDAL projects

## ??? Project Structure

```
PrivacyConfirmedAPI/
??? Controllers/
?   ??? ContactUsController.cs    # API endpoints for Contact Us
??? Properties/
?   ??? launchSettings.json       # Launch configuration
??? appsettings.json              # Application configuration
??? appsettings.Development.json  # Development configuration
??? Program.cs                    # Application startup
??? PrivacyConfirmedAPI.csproj   # Project file
```

## ?? API Endpoints

### Base URL
- Development: `https://localhost:7002`
- HTTP: `http://localhost:5002`

### Endpoints

#### 1. Submit Contact Form
```http
POST /api/contactus
Content-Type: application/json

{
  "name": "John Doe",
  "company": "Tech Solutions Inc",
  "mobileNumber": "9876543210",
  "email": "john.doe@example.com"
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Contact information saved successfully",
  "errors": [],
  "data": {
    "contactEmail": "john.doe@example.com"
  }
}
```

**Response (400 Bad Request):**
```json
{
  "success": false,
  "message": "Validation failed",
  "errors": [
    "Name is required",
    "Email address is not valid"
  ],
  "data": null
}
```

#### 2. Get All Contacts
```http
GET /api/contactus
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Retrieved 5 contact(s)",
  "errors": [],
  "data": [
    {
      "id": 1,
      "name": "John Doe",
      "company": "Tech Solutions Inc",
      "mobileNumber": "9876543210",
      "email": "john.doe@example.com",
      "createdAt": "2024-01-15T10:30:00Z"
    }
  ]
}
```

#### 3. Get Contact by ID
```http
GET /api/contactus/{id}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Contact retrieved successfully",
  "errors": [],
  "data": {
    "id": 1,
    "name": "John Doe",
    "company": "Tech Solutions Inc",
    "mobileNumber": "9876543210",
    "email": "john.doe@example.com",
    "createdAt": "2024-01-15T10:30:00Z"
  }
}
```

**Response (404 Not Found):**
```json
{
  "success": false,
  "message": "Contact with ID 999 not found",
  "errors": []
}
```

#### 4. Health Check
```http
GET /api/contactus/health
```

**Response (200 OK):**
```json
{
  "status": "healthy",
  "timestamp": "2024-01-15T10:30:00Z",
  "version": "1.0.0"
}
```

## ?? Configuration

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=PrivacyConfirmedDB;Trusted_Connection=True;TrustServerCertificate=True;",
    "PostgreSQLConnection": "Host=localhost;Database=privacyconfirmeddb;Username=postgres;Password=your_password;"
  },
  "DatabaseProvider": "SqlServer",
  "Cors": {
    "AllowedOrigins": [
      "https://localhost:7001",
      "http://localhost:5001"
    ]
  }
}
```

### CORS Configuration

The API is configured to allow requests from:
- `https://localhost:7001` (MVC UI HTTPS)
- `http://localhost:5001` (MVC UI HTTP)

To modify allowed origins, update the `AllowMvcUI` policy in `Program.cs`:

```csharp
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
```

## ?? Running the API

### Using Visual Studio
1. Set `PrivacyConfirmedAPI` as the startup project
2. Press F5 or click Run

### Using .NET CLI
```bash
cd PrivacyConfirmedAPI
dotnet run
```

### Using Docker (Optional)
```bash
docker build -t privacyconfirmed-api .
docker run -p 7002:443 -p 5002:80 privacyconfirmed-api
```

## ?? Swagger Documentation

When running in Development mode, Swagger UI is available at the root URL:
- `https://localhost:7002` or `http://localhost:5002`

Swagger provides:
- Interactive API documentation
- Ability to test endpoints directly
- Request/response schemas
- Authentication testing (if implemented)

## ?? Security Considerations

1. **CORS**: Configure allowed origins based on your deployment environment
2. **HTTPS**: Always use HTTPS in production
3. **Authentication**: Consider adding JWT or OAuth authentication for production
4. **Rate Limiting**: Implement rate limiting to prevent abuse
5. **Input Validation**: All inputs are validated using data annotations and service layer validation

## ?? Testing the API

### Using cURL

```bash
# Submit contact
curl -X POST https://localhost:7002/api/contactus \
  -H "Content-Type: application/json" \
  -d '{
    "name": "John Doe",
    "company": "Tech Solutions Inc",
    "mobileNumber": "9876543210",
    "email": "john.doe@example.com"
  }'

# Get all contacts
curl https://localhost:7002/api/contactus

# Get contact by ID
curl https://localhost:7002/api/contactus/1

# Health check
curl https://localhost:7002/api/contactus/health
```

### Using PowerShell

```powershell
# Submit contact
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

# Get all contacts
Invoke-RestMethod -Uri "https://localhost:7002/api/contactus" -Method Get

# Get contact by ID
Invoke-RestMethod -Uri "https://localhost:7002/api/contactus/1" -Method Get

# Health check
Invoke-RestMethod -Uri "https://localhost:7002/api/contactus/health" -Method Get
```

## ?? Integration with MVC UI

To call the API from the MVC UI project:

```csharp
public class ContactUsApiClient
{
    private readonly HttpClient _httpClient;

    public ContactUsApiClient(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
        _httpClient.BaseAddress = new Uri("https://localhost:7002");
    }

    public async Task<ApiResponse> SubmitContactAsync(ContactUsModel model)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/contactus", model);
        return await response.Content.ReadFromJsonAsync<ApiResponse>();
    }
}
```

## ?? Response Format

All API responses follow a consistent format:

```json
{
  "success": true/false,
  "message": "Description of the result",
  "errors": ["List of error messages"],
  "data": { /* Response data */ }
}
```

## ?? Troubleshooting

### CORS Issues
If you encounter CORS errors:
1. Verify the MVC UI URL is in the allowed origins list
2. Check that the API is running on the correct port
3. Ensure the browser is not blocking the request

### Connection String Issues
If database connection fails:
1. Verify SQL Server/PostgreSQL is running
2. Check connection string in appsettings.json
3. Ensure DatabaseProvider is set correctly
4. Verify database and tables exist (run setup scripts)

### Port Already in Use
If port 7002 or 5002 is already in use:
1. Update ports in launchSettings.json
2. Update CORS origins in Program.cs
3. Update any client applications using the API

## ?? Dependencies

- **Microsoft.AspNetCore.OpenApi** (8.0.0) - OpenAPI support
- **Swashbuckle.AspNetCore** (6.5.0) - Swagger UI and documentation
- **PrivacyConfirmedBAL** - Business logic layer
- **PrivacyConfirmedDAL** - Data access layer
- **PrivacyConfirmedModel** - Data models

## ?? License

This project is part of the PrivacyConfirmed application.

## ?? Support

For issues or questions, please contact the development team.
