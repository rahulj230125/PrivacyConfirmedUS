# Resource Center - API-Based Architecture

## ??? Architecture Overview

The Resource Center feature now uses a **clean API-based architecture** where all database interactions go through the PrivacyConfirmedAPI project:

```
???????????????????????????????????????????????????????????????????
?                         USER BROWSER                            ?
???????????????????????????????????????????????????????????????????
                                 ?
                                 ?
???????????????????????????????????????????????????????????????????
?              PrivacyConfirmed MVC Application                   ?
?                   (Presentation Layer)                          ?
?                                                                  ?
?  • ResourceCenterController                                     ?
?  • Views (Index.cshtml)                                         ?
?  • Uses HttpClient to call API                                  ?
???????????????????????????????????????????????????????????????????
                                 ?
                                 ? HTTP/HTTPS
                                 ? (REST API Calls)
                                 ?
???????????????????????????????????????????????????????????????????
?                    PrivacyConfirmedAPI                          ?
?                     (API Layer)                                  ?
?                                                                  ?
?  • ResourceCenterController (API)                               ?
?  • ContactUsController (API)                                    ?
?  • Exposes REST endpoints                                       ?
?  • Handles HTTP requests/responses                              ?
???????????????????????????????????????????????????????????????????
                                 ?
                                 ?
???????????????????????????????????????????????????????????????????
?                PrivacyConfirmedBAL                              ?
?              (Business Logic Layer)                             ?
?                                                                  ?
?  • ResourceFileService                                          ?
?  • ContactUsService                                             ?
?  • Validation & Business Rules                                  ?
???????????????????????????????????????????????????????????????????
                                 ?
                                 ?
???????????????????????????????????????????????????????????????????
?                 PrivacyConfirmedDAL                             ?
?               (Data Access Layer)                               ?
?                                                                  ?
?  • ResourceFileRepository                                       ?
?  • ContactUsRepository                                          ?
?  • Database Operations                                          ?
???????????????????????????????????????????????????????????????????
                                 ?
                                 ?
???????????????????????????????????????????????????????????????????
?                   PostgreSQL Database                           ?
?                                                                  ?
?  • resourcefiles table                                          ?
?  • contactus table                                              ?
?  • Stored Procedures & Functions                                ?
???????????????????????????????????????????????????????????????????
```

---

## ?? API Endpoints

### Resource Center API Endpoints

Base URL: `https://localhost:7002/api/resourcecenter`

| Method | Endpoint | Description | Request | Response |
|--------|----------|-------------|---------|----------|
| GET | `/api/resourcecenter` | Get all files | - | `ApiResponse<List<ResourceFileModel>>` |
| GET | `/api/resourcecenter/{id}` | Get file by ID | Path: `id` | `ApiResponse<ResourceFileModel>` |
| GET | `/api/resourcecenter/{id}/download` | Download file | Path: `id` | File stream |
| POST | `/api/resourcecenter/upload` | Upload file | Form-data: `file` | `ApiResponse` |
| DELETE | `/api/resourcecenter/{id}` | Delete file (soft) | Path: `id` | `ApiResponse` |
| GET | `/api/resourcecenter/health` | Health check | - | Status object |

### Example API Calls

#### Upload File
```http
POST https://localhost:7002/api/resourcecenter/upload
Content-Type: multipart/form-data

file: [binary data]
```

**Response:**
```json
{
  "success": true,
  "message": "File uploaded successfully!",
  "errors": [],
  "data": {
    "fileName": "document.docx"
  }
}
```

#### Get All Files
```http
GET https://localhost:7002/api/resourcecenter
```

**Response:**
```json
{
  "success": true,
  "message": "Retrieved 5 file(s)",
  "errors": [],
  "data": [
    {
      "id": 1,
      "fileName": "document.docx",
      "filePath": "/UploadedFiles/document_20240115120000.docx",
      "fileSize": 524288,
      "fileExtension": ".docx",
      "createdDate": "2024-01-15T12:00:00Z",
      "isDeleted": false
    }
  ]
}
```

#### Delete File
```http
DELETE https://localhost:7002/api/resourcecenter/1
```

**Response:**
```json
{
  "success": true,
  "message": "File deleted successfully",
  "errors": []
}
```

---

## ?? Configuration

### MVC Application (`PrivacyConfirmed/appsettings.json`)

```json
{
  "ApiSettings": {
    "BaseUrl": "https://localhost:7002",
    "Timeout": 30
  }
}
```

### API Application (`PrivacyConfirmedAPI/appsettings.json`)

```json
{
  "ConnectionStrings": {
    "PostgreSQLConnection": "Host=localhost;Database=privacyconfirmedwebsite;Username=webuser;Password=P@ss55word;"
  },
  "DatabaseProvider": "PostgreSQL",
  "FileUploadSettings": {
    "UploadPath": "UploadedFiles",
    "MaxFileSizeInMB": 10,
    "AllowedExtensions": [ ".zip", ".doc", ".docx", ".xlsx", ".xls" ]
  },
  "Cors": {
    "AllowedOrigins": [
      "https://localhost:7001",
      "http://localhost:5001"
    ]
  }
}
```

---

## ?? Running the Application

### Step 1: Start the API
```bash
cd PrivacyConfirmedAPI
dotnet run
```
API will run on: `https://localhost:7002`

### Step 2: Start the MVC App
```bash
cd PrivacyConfirmed
dotnet run
```
MVC will run on: `https://localhost:7001`

### Step 3: Access Resource Center
Open browser: `https://localhost:7001/ResourceCenter`

---

## ?? Benefits of This Architecture

### 1. **Separation of Concerns**
- MVC app focuses on UI/UX
- API handles all data operations
- Clear boundaries between layers

### 2. **Scalability**
- API can be scaled independently
- Multiple clients can use the same API
- Easy to add mobile apps or SPAs

### 3. **Security**
- API-level validation and authentication
- CORS protection
- Centralized security policies

### 4. **Maintainability**
- Changes to database logic isolated in API
- UI changes don't affect data layer
- Easy to test each layer independently

### 5. **Reusability**
- API can serve multiple front-ends
- Consistent data access patterns
- Shared business logic

---

## ?? Security Features

### CORS Configuration
The API is configured to accept requests only from trusted origins:
```csharp
policy.WithOrigins("https://localhost:7001", "http://localhost:5001")
      .AllowAnyHeader()
      .AllowAnyMethod()
      .AllowCredentials();
```

### File Upload Limits
- Maximum file size: 10 MB
- Allowed extensions: `.zip`, `.doc`, `.docx`, `.xlsx`, `.xls`
- Server-side validation

### Anti-Forgery Tokens
All POST/DELETE operations from MVC require CSRF tokens.

---

## ?? Testing the API

### Using Swagger UI
1. Navigate to `https://localhost:7002`
2. Swagger UI loads automatically
3. Test endpoints interactively

### Using HTTP File
Open `PrivacyConfirmedAPI/ResourceCenterAPI.http` in Visual Studio and click "Send Request"

### Using cURL
```bash
# Get all files
curl https://localhost:7002/api/resourcecenter

# Upload file
curl -X POST https://localhost:7002/api/resourcecenter/upload \
  -F "file=@document.docx"

# Delete file
curl -X DELETE https://localhost:7002/api/resourcecenter/1
```

### Using PowerShell
```powershell
# Get all files
Invoke-RestMethod -Uri "https://localhost:7002/api/resourcecenter" -Method Get

# Upload file
$file = Get-Item "document.docx"
$form = @{
    file = $file
}
Invoke-RestMethod -Uri "https://localhost:7002/api/resourcecenter/upload" `
                  -Method Post `
                  -Form $form

# Delete file
Invoke-RestMethod -Uri "https://localhost:7002/api/resourcecenter/1" -Method Delete
```

---

## ?? MVC Controller Flow

The MVC `ResourceCenterController` now acts as a thin client:

```csharp
public async Task<IActionResult> UploadFile(IFormFile file)
{
    // 1. Prepare HTTP request
    var client = _httpClientFactory.CreateClient();
    using var formData = new MultipartFormDataContent();
    // ... prepare form data

    // 2. Call API
    var response = await client.PostAsync($"{_apiBaseUrl}/api/resourcecenter/upload", formData);

    // 3. Handle response
    var apiResponse = JsonSerializer.Deserialize<ApiResponse>(content);

    // 4. Update UI
    TempData["SuccessMessage"] = apiResponse.Message;
    return RedirectToAction(nameof(Index));
}
```

---

## ?? Data Flow Example

### Upload File Flow:

1. **User** selects file in browser ? Submits form
2. **MVC Controller** receives file ? Creates HTTP multipart request
3. **HTTP Call** ? API endpoint `/api/resourcecenter/upload`
4. **API Controller** receives request ? Validates file
5. **Service Layer** saves file to disk ? Creates metadata
6. **Repository** inserts record into database
7. **Database** executes stored procedure ? Returns success
8. **API** returns JSON response
9. **MVC Controller** parses response ? Sets TempData
10. **View** displays success message ? Refreshes grid

---

## ??? Troubleshooting

### Issue: "Unable to connect to API"

**Check:**
1. API is running on `https://localhost:7002`
2. CORS is configured correctly
3. Firewall not blocking port 7002

**Solution:**
```bash
# Verify API is running
curl https://localhost:7002/api/resourcecenter/health

# Check CORS settings in API's Program.cs
```

### Issue: "401 Unauthorized" or CORS errors

**Check:**
1. MVC URL in CORS allowed origins
2. Credentials included in requests

**Solution:**
Update `PrivacyConfirmedAPI/Program.cs`:
```csharp
policy.WithOrigins("https://localhost:7001", "http://localhost:5001")
      .AllowAnyHeader()
      .AllowAnyMethod()
      .AllowCredentials();
```

### Issue: Files not saving

**Check:**
1. `UploadedFiles` folder exists in API project
2. Folder has write permissions
3. Database connection string correct

**Solution:**
```bash
# Create folder in API project root
cd PrivacyConfirmedAPI
mkdir UploadedFiles
```

---

## ?? Project Dependencies

### PrivacyConfirmed (MVC)
```
??? Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation
??? HttpClientFactory (built-in)
??? References: PrivacyConfirmedModel
```

### PrivacyConfirmedAPI
```
??? Microsoft.AspNetCore.OpenApi
??? Swashbuckle.AspNetCore
??? References:
    ??? PrivacyConfirmedModel
    ??? PrivacyConfirmedBAL
    ??? PrivacyConfirmedDAL
```

### PrivacyConfirmedBAL
```
??? Microsoft.AspNetCore.Http.Features
??? Microsoft.Extensions.Logging.Abstractions
??? References:
    ??? PrivacyConfirmedModel
    ??? PrivacyConfirmedDAL
```

### PrivacyConfirmedDAL
```
??? Npgsql
??? Microsoft.Extensions.Configuration
??? References: PrivacyConfirmedModel
```

### PrivacyConfirmedModel
```
??? System.ComponentModel.Annotations
??? Microsoft.AspNetCore.Http.Features
```

---

## ? Deployment Checklist

### API Server
- [ ] Database script executed
- [ ] Connection string configured
- [ ] Upload folder created with write permissions
- [ ] CORS origins updated for production URLs
- [ ] SSL certificate configured
- [ ] Firewall rules for API port
- [ ] Logging configured
- [ ] Health check endpoint tested

### MVC Server
- [ ] API base URL configured in appsettings.json
- [ ] HttpClient timeout configured
- [ ] Error handling tested
- [ ] UI tested with API
- [ ] CSRF tokens working

### Database
- [ ] Table created
- [ ] Stored procedures deployed
- [ ] User permissions granted
- [ ] Backup strategy in place

---

## ?? Performance Considerations

### API Optimization
- Consider adding response caching
- Implement pagination for large file lists
- Use async/await throughout
- Add request rate limiting

### File Storage
- Consider cloud storage (S3, Azure Blob) for production
- Implement CDN for downloads
- Add virus scanning before storage

### Database
- Indexes already created for performance
- Consider read replicas for high load
- Monitor query performance

---

## ?? Next Steps

1. **Add Authentication**
   - Implement JWT tokens
   - Secure API endpoints
   - Add user-specific file access

2. **Add Monitoring**
   - Application Insights
   - Health check dashboard
   - Performance metrics

3. **Enhance Features**
   - File preview
   - Bulk operations
   - File versioning
   - Search and filtering

4. **Cloud Integration**
   - AWS S3 / Azure Blob storage
   - Serverless functions
   - CDN integration

---

## ?? Support

For questions or issues:
1. Check API health: `GET /api/resourcecenter/health`
2. Review logs in both MVC and API projects
3. Test API independently using Swagger
4. Verify database connectivity

---

**Architecture Version:** 2.0 (API-Based)
**Last Updated:** January 2024
