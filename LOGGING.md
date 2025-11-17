# Logging Configuration Guide

## Overview
Both PrivacyConfirmed projects now use **Serilog** for structured, file-based logging. This provides comprehensive logging capabilities with configurable log levels, file rotation, and multiple output targets.

## Features

### 1. **Multiple Log Outputs**
- **Console**: Real-time logs in the console during development
- **Daily Rolling Files**: General logs rotate daily
- **Error-Only Files**: Separate files for errors and critical issues
- **Debug Output**: For Visual Studio debugging

### 2. **Automatic File Management**
- Daily log rotation (one file per day)
- Automatic file size limits (10 MB per file)
- Retention policies:
  - General logs: 30 days
  - Error logs: 90 days
- Automatic cleanup of old logs

### 3. **Structured Logging**
- JSON-compatible log format
- Rich contextual information
- Machine name and thread ID enrichment
- Request logging for HTTP requests

## Log File Locations

### PrivacyConfirmed (Web Application)
```
/Logs/
??? PrivacyConfirmed-20250101.log       (General logs)
??? PrivacyConfirmed-20250102.log
??? PrivacyConfirmed-Errors-20250101.log (Error logs only)
??? PrivacyConfirmed-Errors-20250102.log
```

### PrivacyConfirmedAPI
```
/Logs/
??? PrivacyConfirmedAPI-20250101.log       (General logs)
??? PrivacyConfirmedAPI-20250102.log
??? PrivacyConfirmedAPI-Errors-20250101.log (Error logs only)
??? PrivacyConfirmedAPI-Errors-20250102.log
```

## Configuration

### appsettings.json (Production)
The main configuration is in `appsettings.json`:

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.AspNetCore": "Warning",
        "System": "Warning"
      }
    }
  }
}
```

**Log Levels (from least to most verbose):**
- **Critical**: System failures
- **Error**: Application errors
- **Warning**: Potential issues
- **Information**: General information (default)
- **Debug**: Detailed diagnostic info
- **Verbose/Trace**: Very detailed tracing

### appsettings.Development.json (Development)
Development environment uses more verbose logging:

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Debug"
    }
  }
}
```

## Customization

### Change Log Levels
Edit `appsettings.json` to adjust verbosity:

```json
"Serilog": {
  "MinimumLevel": {
    "Default": "Debug",  // Change to: Error, Warning, Information, Debug
    "Override": {
      "Microsoft.EntityFrameworkCore": "Information"  // Add specific overrides
    }
  }
}
```

### Change File Retention
Modify the `retainedFileCountLimit` in `appsettings.json`:

```json
"WriteTo": [
  {
    "Name": "File",
    "Args": {
      "path": "Logs/PrivacyConfirmed-.log",
      "retainedFileCountLimit": 60  // Keep 60 days instead of 30
    }
  }
]
```

### Change File Size Limit
Adjust `fileSizeLimitBytes` (in bytes):

```json
"Args": {
  "fileSizeLimitBytes": 52428800  // 50 MB instead of 10 MB
}
```

### Change Log Location
Update the `path` parameter:

```json
"Args": {
  "path": "D:/MyLogs/App-.log"  // Custom path
}
```

## Usage in Code

The existing code already uses ILogger, which now writes to files automatically:

```csharp
public class ContactUsController : ControllerBase
{
    private readonly ILogger<ContactUsController> _logger;

    public ContactUsController(ILogger<ContactUsController> logger)
    {
        _logger = logger;
    }

    public async Task<IActionResult> SubmitContact(ContactUsModel model)
    {
        _logger.LogInformation("Processing contact from {Email}", model.Email);
        
        try
        {
            // Business logic
            _logger.LogDebug("Contact saved with ID {Id}", contactId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save contact");
        }
    }
}
```

## Log Format

### Console Output
```
[2025-01-15 10:30:45.123 +00:00] [INF] PrivacyConfirmedAPI.Controllers.ContactUsController
Processing contact from user@example.com
```

### File Output
```
[2025-01-15 10:30:45.123 +00:00] [INF] [PrivacyConfirmedAPI.Controllers.ContactUsController] Processing contact from user@example.com
[2025-01-15 10:30:45.456 +00:00] [ERR] [PrivacyConfirmedAPI.Controllers.ContactUsController] Failed to save contact
System.Exception: Database connection failed
   at PrivacyConfirmedAPI.Controllers.ContactUsController.SubmitContact()
```

## Request Logging

HTTP requests are automatically logged with:
- Request method and path
- Status code
- Response time
- User information (if authenticated)

Example:
```
[2025-01-15 10:30:45.789 +00:00] [INF] HTTP POST /api/contactus responded 200 in 234.5678 ms
```

## Environment-Specific Configuration

### Development
- Log level: Debug
- More verbose Microsoft logs
- Full request/response details

### Production
- Log level: Information
- Reduced Microsoft logs (Warning only)
- Optimized for performance

## Best Practices

1. **Use appropriate log levels**:
   - `LogError`: For exceptions and errors
   - `LogWarning`: For concerning but non-critical issues
   - `LogInformation`: For important business events
   - `LogDebug`: For detailed diagnostics (development only)

2. **Include context in messages**:
   ```csharp
   _logger.LogInformation("User {UserId} uploaded file {FileName}", userId, fileName);
   ```

3. **Don't log sensitive data**:
   ```csharp
   // BAD
   _logger.LogInformation("Password: {Password}", password);
   
   // GOOD
   _logger.LogInformation("User authentication attempt for {Username}", username);
   ```

4. **Use structured logging**:
   ```csharp
   // Good - structured properties
   _logger.LogInformation("Processing order {OrderId} for customer {CustomerId}", orderId, customerId);
   
   // Avoid - string concatenation
   _logger.LogInformation("Processing order " + orderId + " for customer " + customerId);
   ```

## Monitoring Logs

### View Latest Logs
```bash
# Windows
type Logs\PrivacyConfirmed-20250115.log

# Linux/Mac
tail -f Logs/PrivacyConfirmed-20250115.log
```

### Search for Errors
```bash
# Windows
findstr /i "ERR" Logs\PrivacyConfirmed-Errors-20250115.log

# Linux/Mac
grep -i "ERR" Logs/PrivacyConfirmed-Errors-20250115.log
```

### Filter by Time
```bash
# Windows
findstr /i "10:30" Logs\PrivacyConfirmed-20250115.log

# Linux/Mac
grep "10:30" Logs/PrivacyConfirmed-20250115.log
```

## Troubleshooting

### Logs Not Being Created
1. Check file permissions on the Logs directory
2. Verify the application has write access
3. Check for exceptions during startup in console output

### Log Files Too Large
1. Reduce `fileSizeLimitBytes` in configuration
2. Decrease `retainedFileCountLimit`
3. Change log level to Information or Warning

### Missing Log Entries
1. Verify log level configuration
2. Check if the source is being filtered in Override section
3. Ensure `Log.CloseAndFlush()` is called on shutdown

## Additional Resources

- [Serilog Documentation](https://serilog.net/)
- [ASP.NET Core Logging](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/)
- [Structured Logging Best Practices](https://github.com/serilog/serilog/wiki/Structured-Data)
