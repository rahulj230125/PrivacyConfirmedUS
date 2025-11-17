# Quick Start: Logging Configuration

## What Was Changed

? **Serilog** has been added to both projects (PrivacyConfirmed and PrivacyConfirmedAPI)  
? **File-based logging** is now active with automatic file rotation  
? **Configuration** is fully managed through appsettings.json  
? **No code changes required** - existing `ILogger` usage works automatically

## Where Are My Logs?

Both projects create log files in their respective `Logs/` directories:

```
PrivacyConfirmed/
  ??? Logs/
      ??? PrivacyConfirmed-20250115.log         (All logs)
      ??? PrivacyConfirmed-Errors-20250115.log  (Errors only)

PrivacyConfirmedAPI/
  ??? Logs/
      ??? PrivacyConfirmedAPI-20250115.log         (All logs)
      ??? PrivacyConfirmedAPI-Errors-20250115.log  (Errors only)
```

## Quick Configuration Changes

### Change Log Level (appsettings.json)

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information"  // Change to: Debug, Information, Warning, Error
    }
  }
}
```

**Common Scenarios:**
- **Production**: "Information" or "Warning"
- **Development**: "Debug"
- **Troubleshooting**: "Debug" or "Verbose"

### Change Log Retention

```json
{
  "Serilog": {
    "WriteTo": [
      {
        "Name": "File",
        "Args": {
          "retainedFileCountLimit": 30  // Days to keep logs
        }
      }
    ]
  }
}
```

### Change File Size Limit

```json
{
  "Serilog": {
    "WriteTo": [
      {
        "Name": "File",
        "Args": {
          "fileSizeLimitBytes": 10485760  // 10 MB (in bytes)
        }
      }
    ]
  }
}
```

## Common Log Levels

| Level | When to Use | Example |
|-------|-------------|---------|
| **Error** | Exceptions, failures | `_logger.LogError(ex, "Failed to save")` |
| **Warning** | Concerning but not critical | `_logger.LogWarning("Retry attempt {Attempt}", 3)` |
| **Information** | Important business events | `_logger.LogInformation("User {User} logged in", user)` |
| **Debug** | Detailed diagnostics | `_logger.LogDebug("Processing item {Id}", id)` |

## How to View Logs

### Real-Time Monitoring (PowerShell)
```powershell
Get-Content -Path "Logs\PrivacyConfirmed-$(Get-Date -Format 'yyyyMMdd').log" -Wait -Tail 50
```

### View Errors Only
```powershell
Get-Content -Path "Logs\PrivacyConfirmed-Errors-$(Get-Date -Format 'yyyyMMdd').log"
```

### Search for Specific Text
```powershell
Select-String -Path "Logs\*.log" -Pattern "YourSearchTerm"
```

## Environment-Specific Settings

### Development (appsettings.Development.json)
- More verbose logging (Debug level)
- Includes detailed framework logs

### Production (appsettings.json)
- Less verbose (Information level)
- Filtered Microsoft framework logs
- Optimized for performance

## Files Modified

1. **PrivacyConfirmed/Program.cs** - Added Serilog configuration
2. **PrivacyConfirmedAPI/Program.cs** - Added Serilog configuration
3. **PrivacyConfirmed/appsettings.json** - Added Serilog settings
4. **PrivacyConfirmedAPI/appsettings.json** - Added Serilog settings
5. **PrivacyConfirmed/appsettings.Development.json** - Development log settings
6. **PrivacyConfirmedAPI/appsettings.Development.json** - Development log settings
7. **PrivacyConfirmed.csproj** - Added Serilog.AspNetCore package
8. **PrivacyConfirmedAPI.csproj** - Added Serilog.AspNetCore package
9. **.gitignore** - Excluded Logs/ directory

## No Action Required

? All existing logging code continues to work  
? Log files are automatically created on first run  
? Old logs are automatically cleaned up  
? File rotation happens automatically at midnight  

## Next Steps

1. **Run the application** - Logs will be created automatically
2. **Check the Logs folder** - Files appear after first log entry
3. **Adjust settings** - Modify appsettings.json as needed
4. **Monitor logs** - Use the PowerShell commands above

## Troubleshooting

**Q: No log files are created**  
A: Ensure the application has write permissions for the Logs directory. Check console output for errors.

**Q: Too many log files**  
A: Reduce `retainedFileCountLimit` in appsettings.json

**Q: Log files too large**  
A: Reduce `fileSizeLimitBytes` or change log level to Warning/Error only

**Q: Missing logs in files but see them in console**  
A: Check file write permissions and verify Serilog configuration in appsettings.json

## Learn More

See [LOGGING.md](./LOGGING.md) for comprehensive documentation.
