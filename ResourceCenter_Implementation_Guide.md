# Resource Center Feature - Implementation Guide

## Overview
The Resource Center is a complete file management system that allows users to upload, view, download, and delete documents. It supports ZIP archives, Word documents (.doc, .docx), and Excel spreadsheets (.xlsx, .xls).

---

## ?? Features Implemented

### 1. **File Upload System**
- ? Accepts only specific file types: `.zip`, `.doc`, `.docx`, `.xlsx`, `.xls`
- ? Maximum file size limit: 10 MB
- ? Files saved to configurable local folder
- ? Unique filename generation to prevent overwrites
- ? Comprehensive validation (file type, size, null checks)

### 2. **File Management Grid**
- ? Bootstrap 5 responsive table
- ? Displays: File Name, Type, Size, Upload Date, Status
- ? Action buttons: Download and Delete
- ? Sortable columns
- ? File statistics dashboard

### 3. **Database Integration**
- ? PostgreSQL stored procedures for CRUD operations
- ? Soft delete functionality (IsDeleted flag)
- ? Full metadata storage

### 4. **User Interface**
- ? Modern Bootstrap 5 design
- ? Responsive layout
- ? Success/Error notifications
- ? Confirmation modal for deletions
- ? File type badges with color coding
- ? Statistics dashboard

---

## ??? Files Created

### Database Scripts
```
DatabaseScripts/PostgreSQL_ResourceCenter_Setup.sql
```
- Creates `resourcefiles` table
- Stored procedures: `sp_insert_resourcefile`, `sp_delete_resourcefile`
- Functions: `get_all_resourcefiles`, `get_resourcefile_by_id`
- Indexes for performance optimization

### Models
```
PrivacyConfirmedModel/ResourceFileModel.cs
```
- `ResourceFileModel` - Main entity model
- `FileUploadViewModel` - View model for upload page
- Helper methods for file size formatting

### Data Access Layer (DAL)
```
PrivacyConfirmedDAL/Interfaces/IResourceFileRepository.cs
PrivacyConfirmedDAL/Repositories/ResourceFileRepository.cs
```
- Repository pattern implementation
- PostgreSQL-specific data access
- CRUD operations with stored procedures

### Business Logic Layer (BAL)
```
PrivacyConfirmedBAL/Interfaces/IResourceFileService.cs
PrivacyConfirmedBAL/Services/ResourceFileService.cs
```
- File validation logic
- File upload/download business rules
- Soft delete implementation
- Error handling and logging

### Controller
```
PrivacyConfirmed/Controllers/ResourceCenterController.cs
```
- `Index` - Display grid and upload form
- `UploadFile` - Handle file uploads
- `Download` - Stream file to user
- `Delete` - Soft delete file

### Views
```
PrivacyConfirmed/Views/ResourceCenter/Index.cshtml
```
- File upload form with drag-and-drop styling
- Responsive data grid
- Statistics dashboard
- Delete confirmation modal
- Client-side validation

---

## ?? Setup Instructions

### Step 1: Run Database Script

```bash
# Connect to PostgreSQL
psql -U webuser -d privacyconfirmedwebsite

# Run the setup script
\i DatabaseScripts/PostgreSQL_ResourceCenter_Setup.sql

# Or using psql command line:
psql -U webuser -d privacyconfirmedwebsite -f DatabaseScripts/PostgreSQL_ResourceCenter_Setup.sql
```

### Step 2: Grant Permissions

```sql
-- Grant necessary permissions to webuser
GRANT SELECT, INSERT, UPDATE ON resourcefiles TO webuser;
GRANT EXECUTE ON PROCEDURE sp_insert_resourcefile TO webuser;
GRANT EXECUTE ON PROCEDURE sp_delete_resourcefile TO webuser;
GRANT EXECUTE ON FUNCTION get_all_resourcefiles TO webuser;
GRANT EXECUTE ON FUNCTION get_resourcefile_by_id TO webuser;
GRANT USAGE, SELECT ON SEQUENCE resourcefiles_id_seq TO webuser;
```

### Step 3: Configure Upload Path

The upload path is configured in `appsettings.json`:

```json
{
  "FileUploadSettings": {
    "UploadPath": "UploadedFiles",
    "MaxFileSizeInMB": 10,
    "AllowedExtensions": [ ".zip", ".doc", ".docx", ".xlsx", ".xls" ]
  }
}
```

**Options:**
- **Relative path**: `"UploadPath": "UploadedFiles"` ? Creates folder in project root
- **Absolute path**: `"UploadPath": "C:\\FileStorage\\UploadedFiles"` ? Uses specific directory
- **Web root path**: Files in `wwwroot/UploadedFiles` are publicly accessible

### Step 4: Create Upload Directory

```bash
# Windows
mkdir UploadedFiles

# Linux/Mac
mkdir -p UploadedFiles
chmod 755 UploadedFiles
```

### Step 5: Restore Packages and Build

```bash
# Restore NuGet packages
dotnet restore

# Build the solution
dotnet build

# Run the application
dotnet run --project PrivacyConfirmed
```

---

## ?? Database Schema

### Table: resourcefiles

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| id | SERIAL | PRIMARY KEY | Unique identifier |
| filename | VARCHAR(255) | NOT NULL | Original file name |
| filepath | VARCHAR(500) | NOT NULL | Full path on server |
| filesize | BIGINT | NOT NULL, DEFAULT 0 | File size in bytes |
| fileextension | VARCHAR(50) | NOT NULL | File extension |
| createddate | TIMESTAMP | NOT NULL, DEFAULT UTC | Upload timestamp |
| isdeleted | BOOLEAN | NOT NULL, DEFAULT FALSE | Soft delete flag |

### Indexes
- `idx_resourcefiles_createddate` - Optimizes sorting by date
- `idx_resourcefiles_isdeleted` - Optimizes filtering deleted files
- `idx_resourcefiles_filename` - Optimizes searching by filename

---

## ?? Usage Guide

### Accessing Resource Center

1. **Via Navigation Menu**
   - Click "Resources" in the top navigation
   - Select "Resource Center" from dropdown

2. **Direct URL**
   - Navigate to: `/ResourceCenter` or `/ResourceCenter/Index`

### Uploading Files

1. Click the file input or drag files to the upload area
2. Select a file (only allowed types shown)
3. Click "Upload File" button
4. Success/error message displayed
5. File appears in the grid below

### Downloading Files

1. Locate the file in the grid
2. Click the "Download" button
3. File downloads to your browser's default location

### Deleting Files

1. Click the "Delete" button for a file
2. Confirmation modal appears
3. Confirm deletion
4. File marked as deleted (soft delete)
5. File removed from grid display

---

## ?? Security Considerations

### File Upload Security

1. **File Type Validation**
   - Only allowed extensions accepted
   - Validated on both client and server side

2. **File Size Limits**
   - Maximum 10 MB per file
   - Configurable in settings

3. **Unique Filenames**
   - Timestamp appended to prevent overwrites
   - Prevents path traversal attacks

4. **Anti-Forgery Tokens**
   - All POST requests protected with CSRF tokens

### Recommended Enhancements for Production

1. **Virus Scanning**
   ```csharp
   // Add antivirus scanning before saving
   var scanResult = await _antivirusService.ScanFileAsync(file);
   if (!scanResult.IsClean)
   {
       return "File failed security scan";
   }
   ```

2. **User Authentication**
   ```csharp
   [Authorize] // Require login
   public class ResourceCenterController : Controller
   ```

3. **Storage Limits**
   - Implement per-user storage quotas
   - Monitor total storage usage

4. **File Content Validation**
   - Verify actual file content matches extension
   - Check for embedded malicious code

---

## ?? Customization

### Changing Allowed File Types

**In appsettings.json:**
```json
"FileUploadSettings": {
  "AllowedExtensions": [ ".pdf", ".png", ".jpg", ".txt" ]
}
```

**In Service:**
```csharp
private static readonly string[] AllowedExtensions = 
    { ".pdf", ".png", ".jpg", ".txt" };
```

### Changing Maximum File Size

**In appsettings.json:**
```json
"FileUploadSettings": {
  "MaxFileSizeInMB": 50
}
```

**In Service:**
```csharp
private const long MaxFileSize = 50 * 1024 * 1024; // 50 MB
```

### Customizing UI Colors

Edit the `<style>` section in `Index.cshtml`:

```css
.resource-center-header {
    background: linear-gradient(135deg, #your-color-1 0%, #your-color-2 100%);
}

.file-stats {
    background: linear-gradient(135deg, #your-color-1 0%, #your-color-2 100%);
}
```

---

## ?? Testing

### Manual Testing Checklist

- [ ] Upload valid file types (.zip, .doc, .docx, .xlsx, .xls)
- [ ] Try uploading invalid file type (.exe, .bat)
- [ ] Upload file exceeding size limit
- [ ] Upload file with special characters in name
- [ ] Download uploaded file
- [ ] Delete file and verify it's removed from grid
- [ ] Check file exists on server after upload
- [ ] Verify database record created
- [ ] Test responsive layout on mobile
- [ ] Test with no files uploaded

### Test Data

Run these SQL queries to insert test data:

```sql
-- Insert test files
CALL sp_insert_resourcefile(
    'Project_Documentation.docx',
    '/UploadedFiles/Project_Documentation_20240115120000.docx',
    524288,
    '.docx',
    NOW() AT TIME ZONE 'UTC'
);

CALL sp_insert_resourcefile(
    'Data_Export.xlsx',
    '/UploadedFiles/Data_Export_20240115120100.xlsx',
    1048576,
    '.xlsx',
    NOW() AT TIME ZONE 'UTC'
);

-- Verify insertion
SELECT * FROM get_all_resourcefiles();
```

---

## ?? Troubleshooting

### Issue: Files not uploading

**Check:**
1. Upload directory exists and has write permissions
2. File size within limit
3. File extension is allowed
4. Database connection string correct

**Solution:**
```bash
# Check directory permissions (Linux/Mac)
ls -la UploadedFiles

# Grant write permissions
chmod 755 UploadedFiles
```

### Issue: Database errors

**Check:**
1. PostgreSQL service running
2. User has correct permissions
3. Database exists
4. Stored procedures created

**Solution:**
```sql
-- Verify stored procedures exist
\df sp_insert_resourcefile

-- Check user permissions
\du webuser
```

### Issue: Download not working

**Check:**
1. Physical file exists at stored path
2. File path in database is correct
3. Application has read permissions

**Solution:**
```csharp
// Add logging to download action
_logger.LogInformation("Attempting download: {FilePath}", file.FilePath);
if (!System.IO.File.Exists(file.FilePath))
{
    _logger.LogError("File not found: {FilePath}", file.FilePath);
}
```

---

## ?? Performance Optimization

### Database Indexes

Already created in setup script:
- Index on `createddate` for fast sorting
- Index on `isdeleted` for filtering
- Index on `filename` for searching

### Caching (Optional)

Add caching to reduce database calls:

```csharp
// In Controller
private readonly IMemoryCache _cache;

public async Task<IActionResult> Index()
{
    var cacheKey = "resourcefiles_list";
    
    if (!_cache.TryGetValue(cacheKey, out List<ResourceFileModel> files))
    {
        files = await _resourceFileService.GetAllFilesAsync();
        
        _cache.Set(cacheKey, files, TimeSpan.FromMinutes(5));
    }
    
    return View(new FileUploadViewModel { ResourceFiles = files });
}
```

### Pagination (For Large Lists)

Implement pagination if you expect many files:

```csharp
public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
{
    var files = await _resourceFileService.GetPagedFilesAsync(page, pageSize);
    return View(files);
}
```

---

## ?? Future Enhancements

### Planned Features

1. **Search and Filter**
   - Search by filename
   - Filter by file type
   - Date range filtering

2. **Bulk Operations**
   - Select multiple files
   - Bulk download as ZIP
   - Bulk delete

3. **File Preview**
   - Preview documents in browser
   - Thumbnail generation for images

4. **Version Control**
   - Track file versions
   - Allow file replacement
   - Version history

5. **Sharing**
   - Generate shareable links
   - Set expiration dates
   - Access control

6. **Cloud Storage Integration**
   - AWS S3 support
   - Azure Blob Storage
   - Google Cloud Storage

---

## ?? Support

For issues or questions:
1. Check this documentation
2. Review error logs in `logs/` directory
3. Check database logs
4. Contact development team

---

## ? Checklist for Production Deployment

- [ ] Database script executed
- [ ] Permissions granted to database user
- [ ] Upload directory created with correct permissions
- [ ] Configuration updated in appsettings.json
- [ ] SSL/HTTPS enabled
- [ ] Authentication and authorization implemented
- [ ] File upload limits configured at server level (IIS/Kestrel)
- [ ] Monitoring and logging configured
- [ ] Backup strategy for uploaded files
- [ ] Antivirus scanning integrated
- [ ] Storage quota management implemented
- [ ] Error handling tested
- [ ] Load testing completed

---

## ?? License

This feature is part of the PrivacyConfirmed application.

---

**Last Updated:** January 2024
**Version:** 1.0.0
