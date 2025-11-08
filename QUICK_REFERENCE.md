# Resource Center - Quick Reference Card

## ?? Start Commands

```bash
# Terminal 1 - Start API
cd PrivacyConfirmedAPI
dotnet run
# ? https://localhost:7002

# Terminal 2 - Start MVC
cd PrivacyConfirmed
dotnet run
# ? https://localhost:7001
```

## ?? API Endpoints Quick Reference

```
GET    /api/resourcecenter           ? Get all files
GET    /api/resourcecenter/{id}      ? Get file by ID
GET    /api/resourcecenter/{id}/download ? Download file
POST   /api/resourcecenter/upload    ? Upload file
DELETE /api/resourcecenter/{id}      ? Delete file
GET    /api/resourcecenter/health    ? Health check
```

## ?? URLs

| Service | URL | Description |
|---------|-----|-------------|
| MVC App | https://localhost:7001 | Main application |
| Resource Center | https://localhost:7001/ResourceCenter | File management page |
| API | https://localhost:7002 | REST API |
| Swagger UI | https://localhost:7002 | API documentation |

## ?? Configuration Files

| File | Key Setting | Value |
|------|-------------|-------|
| `PrivacyConfirmed/appsettings.json` | ApiSettings:BaseUrl | https://localhost:7002 |
| `PrivacyConfirmedAPI/appsettings.json` | DatabaseProvider | PostgreSQL |
| `PrivacyConfirmedAPI/appsettings.json` | FileUploadSettings:UploadPath | UploadedFiles |
| `PrivacyConfirmedAPI/appsettings.json` | FileUploadSettings:MaxFileSizeInMB | 10 |

## ?? Database Commands

```sql
-- View all files
SELECT * FROM resourcefiles WHERE isdeleted = FALSE;

-- Count files
SELECT COUNT(*) FROM resourcefiles WHERE isdeleted = FALSE;

-- Grant permissions
GRANT SELECT, INSERT, UPDATE ON resourcefiles TO webuser;
GRANT USAGE, SELECT ON SEQUENCE resourcefiles_id_seq TO webuser;
GRANT EXECUTE ON ALL PROCEDURES IN SCHEMA public TO webuser;
GRANT EXECUTE ON ALL FUNCTIONS IN SCHEMA public TO webuser;
```

## ?? Quick Tests

### Test API (PowerShell)
```powershell
# Health check
Invoke-RestMethod "https://localhost:7002/api/resourcecenter/health"

# Get all files
Invoke-RestMethod "https://localhost:7002/api/resourcecenter"

# Upload file
$file = Get-Item "test.zip"
$form = @{ file = $file }
Invoke-RestMethod -Uri "https://localhost:7002/api/resourcecenter/upload" -Method Post -Form $form
```

### Test API (cURL)
```bash
# Health check
curl https://localhost:7002/api/resourcecenter/health

# Get all files
curl https://localhost:7002/api/resourcecenter

# Upload file
curl -X POST https://localhost:7002/api/resourcecenter/upload -F "file=@test.zip"

# Delete file
curl -X DELETE https://localhost:7002/api/resourcecenter/1
```

## ?? Common Issues

| Issue | Quick Fix |
|-------|-----------|
| API won't start | Check port 7002, change in launchSettings.json if needed |
| CORS error | Verify MVC URL in API's CORS settings |
| Files won't upload | Create `UploadedFiles` folder in API root |
| Database error | Check connection string, verify permissions |
| MVC can't reach API | Verify API is running, check ApiSettings:BaseUrl |

## ?? Project Structure

```
PrivacyConfirmed/
??? Controllers/
?   ??? ResourceCenterController.cs  ? HTTP client calls
??? Views/
?   ??? ResourceCenter/
?       ??? Index.cshtml              ? UI
??? appsettings.json                  ? API URL config

PrivacyConfirmedAPI/
??? Controllers/
?   ??? ResourceCenterController.cs  ? REST API endpoints
??? UploadedFiles/                    ? File storage
??? appsettings.json                  ? DB config

PrivacyConfirmedBAL/
??? Services/
    ??? ResourceFileService.cs        ? Business logic

PrivacyConfirmedDAL/
??? Repositories/
    ??? ResourceFileRepository.cs     ? Database access

PrivacyConfirmedModel/
??? ResourceFileModel.cs              ? Data models

DatabaseScripts/
??? PostgreSQL_ResourceCenter_Setup.sql ? DB setup
```

## ?? Data Flow

```
User ? MVC Controller ? HttpClient ? API Controller ? Service ? Repository ? Database
                                      ?
                               FileSystem (UploadedFiles/)
```

## ? Pre-Flight Checklist

Before running:
- [ ] PostgreSQL running
- [ ] Database `privacyconfirmedwebsite` exists
- [ ] Database script executed
- [ ] Permissions granted to `webuser`
- [ ] `UploadedFiles` folder created in API project
- [ ] .NET 8 SDK installed

## ?? Quick Help

### Get Build Errors
```bash
dotnet build
```

### Restore Packages
```bash
dotnet restore
```

### Clear and Rebuild
```bash
dotnet clean
dotnet restore
dotnet build
```

### View Logs
- API logs: Check console output
- MVC logs: Check console output
- Database logs: Check PostgreSQL logs

## ?? Key Files to Remember

| Task | File to Edit |
|------|--------------|
| Change API URL | `PrivacyConfirmed/appsettings.json` |
| Change DB connection | `PrivacyConfirmedAPI/appsettings.json` |
| Change file size limit | `PrivacyConfirmedAPI/appsettings.json` |
| Change upload folder | `PrivacyConfirmedAPI/appsettings.json` |
| Add file types | `PrivacyConfirmedBAL/Services/ResourceFileService.cs` |
| Modify UI | `PrivacyConfirmed/Views/ResourceCenter/Index.cshtml` |

## ?? Documentation Files

1. `RESOURCE_CENTER_COMPLETE.md` - Complete summary
2. `ResourceCenter_API_Architecture.md` - Architecture details
3. `ResourceCenter_Implementation_Guide.md` - Setup guide
4. `ResourceCenter_QuickStart.md` - Quick setup
5. `ResourceCenterAPI.http` - API test file

## ?? Success Indicators

? API runs on 7002
? MVC runs on 7001
? Swagger UI loads
? Resource Center menu appears
? Can upload files
? Files appear in grid
? Can download files
? Can delete files
? Database records created

---

**Keep this card handy for quick reference!**
