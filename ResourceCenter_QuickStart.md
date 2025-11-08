# Resource Center - Quick Setup Guide

## ?? 5-Minute Setup

### Step 1: Run Database Script (1 min)
```bash
psql -U webuser -d privacyconfirmedwebsite -f DatabaseScripts/PostgreSQL_ResourceCenter_Setup.sql
```

### Step 2: Grant Permissions (30 sec)
```sql
GRANT SELECT, INSERT, UPDATE ON resourcefiles TO webuser;
GRANT EXECUTE ON ALL PROCEDURES IN SCHEMA public TO webuser;
GRANT EXECUTE ON ALL FUNCTIONS IN SCHEMA public TO webuser;
GRANT USAGE, SELECT ON SEQUENCE resourcefiles_id_seq TO webuser;
```

### Step 3: Create Upload Folder (30 sec)
```bash
mkdir UploadedFiles
# Or on Windows: md UploadedFiles
```

### Step 4: Build and Run (3 min)
```bash
dotnet restore
dotnet build
dotnet run --project PrivacyConfirmed
```

### Step 5: Test It! (30 sec)
1. Open browser: `https://localhost:7001`
2. Click "Resources" ? "Resource Center"
3. Upload a test file
4. ? Done!

---

## ?? What Was Added

### New Files
```
? ResourceFileModel.cs              - Model
? IResourceFileRepository.cs        - Repository Interface
? ResourceFileRepository.cs         - Repository Implementation
? IResourceFileService.cs           - Service Interface
? ResourceFileService.cs            - Service Implementation
? ResourceCenterController.cs       - Controller
? Index.cshtml                      - View
? PostgreSQL_ResourceCenter_Setup.sql - Database Script
```

### Modified Files
```
? _Layout.cshtml                    - Added menu link
? Program.cs                        - Registered services
? appsettings.json                  - Added file upload config
? PrivacyConfirmedModel.csproj     - Added package reference
? PrivacyConfirmedBAL.csproj       - Added package reference
```

---

## ?? Quick Test

### Upload a File
```
1. Navigate to: /ResourceCenter
2. Click "Choose a file"
3. Select any .zip, .doc, .docx, .xlsx file
4. Click "Upload File"
5. File appears in grid below
```

### Download a File
```
1. Find file in grid
2. Click "Download" button
3. File downloads to browser
```

### Delete a File
```
1. Find file in grid
2. Click "Delete" button
3. Confirm in modal
4. File removed from grid
```

---

## ?? Configuration

### Upload Path (appsettings.json)
```json
"FileUploadSettings": {
  "UploadPath": "UploadedFiles",           // Folder path
  "MaxFileSizeInMB": 10,                   // Max file size
  "AllowedExtensions": [".zip", ".doc", ".docx", ".xlsx", ".xls"]
}
```

### Database (appsettings.json)
```json
"ConnectionStrings": {
  "PostgreSQLConnection": "Host=localhost;Database=privacyconfirmedwebsite;Username=webuser;Password=P@ss55word;"
},
"DatabaseProvider": "PostgreSQL"
```

---

## ?? Verify Everything Works

### Check Database
```sql
-- View all files
SELECT * FROM resourcefiles;

-- Count files
SELECT COUNT(*) FROM resourcefiles WHERE isdeleted = FALSE;

-- Check stored procedures
\df sp_insert_resourcefile
\df sp_delete_resourcefile
```

### Check File System
```bash
# List uploaded files
ls -la UploadedFiles/

# Check permissions (Linux/Mac)
stat UploadedFiles
```

### Check Application
```bash
# View logs
tail -f logs/app.log

# Check build
dotnet build --configuration Release
```

---

## ?? Quick Troubleshooting

| Problem | Solution |
|---------|----------|
| **Can't upload files** | Check folder exists and has write permissions |
| **Database error** | Verify connection string and user permissions |
| **File not found** | Check UploadPath in appsettings.json |
| **Build errors** | Run `dotnet restore` and rebuild |
| **403 Forbidden** | Check file/folder permissions |

---

## ?? Database Tables

### resourcefiles
- `id` - Auto-incrementing primary key
- `filename` - Original file name
- `filepath` - Full path on server
- `filesize` - Size in bytes
- `fileextension` - File extension (.zip, .doc, etc.)
- `createddate` - Upload timestamp (UTC)
- `isdeleted` - Soft delete flag

---

## ?? Customization Quick Tips

### Change Max File Size
```csharp
// ResourceFileService.cs
private const long MaxFileSize = 20 * 1024 * 1024; // 20 MB
```

### Add More File Types
```csharp
// ResourceFileService.cs
private static readonly string[] AllowedExtensions = 
    { ".zip", ".doc", ".docx", ".xlsx", ".xls", ".pdf", ".txt" };
```

### Change UI Colors
```css
/* Index.cshtml */
.resource-center-header {
    background: linear-gradient(135deg, #your-color-1 0%, #your-color-2 100%);
}
```

---

## ? Pre-Deployment Checklist

- [ ] Database script executed successfully
- [ ] All database permissions granted
- [ ] Upload folder created with correct permissions
- [ ] Configuration updated in appsettings.json
- [ ] Solution builds without errors
- [ ] Upload test completed successfully
- [ ] Download test completed successfully
- [ ] Delete test completed successfully
- [ ] Menu link appears correctly
- [ ] Error handling tested

---

## ?? Need Help?

1. Check `ResourceCenter_Implementation_Guide.md` for detailed documentation
2. Review error logs in the application
3. Check PostgreSQL logs: `tail -f /var/log/postgresql/*.log`
4. Verify all services registered in `Program.cs`

---

## ?? Success Indicators

? Menu shows "Resource Center" under Resources
? Page loads without errors
? File upload works
? Files appear in grid
? Download works
? Delete works
? Database records created

---

**Ready to go! Your Resource Center is fully functional.** ??
