# Contact Us Feature - Implementation Guide

## Overview
This document provides comprehensive information about the Contact Us feature implementation for PrivacyConfirmed IAM platform.

## Solution Structure

The solution follows **Clean Architecture** principles with three separate projects:

```
PrivacyConfirmed (Root)
??? PrivacyConfirmedModel/          # Data Models with validation
??? PrivacyConfirmedDAL/           # Data Access Layer
??? PrivacyConfirmedBAL/           # Business Access Layer
??? PrivacyConfirmed/              # Main Web Application
??? DatabaseScripts/               # SQL Scripts
```

---

## Project Details

### 1. PrivacyConfirmedModel
**Purpose**: Contains data models with validation attributes

**Files**:
- `ContactUsModel.cs` - Contact form model with Data Annotations

**Key Features**:
- Input validation using Data Annotations
- Required field validation
- String length validation
- Email format validation
- Phone number regex validation (10 digits)

### 2. PrivacyConfirmedDAL (Data Access Layer)
**Purpose**: Handles all database operations

**Files**:
- `Interfaces/IContactUsRepository.cs` - Repository interface
- `Repositories/ContactUsRepository.cs` - Repository implementation

**Key Features**:
- Support for both SQL Server and PostgreSQL
- Stored procedure execution
- Connection string management
- Database provider configuration
- Async operations for better performance

### 3. PrivacyConfirmedBAL (Business Access Layer)
**Purpose**: Contains business logic and validation

**Files**:
- `Interfaces/IContactUsService.cs` - Service interface with ServiceResult class
- `Services/ContactUsService.cs` - Service implementation

**Key Features**:
- Business rule validation
- Data sanitization
- Duplicate email checking
- Error handling and logging
- Result object pattern

### 4. PrivacyConfirmed (Web Application)
**Purpose**: Main ASP.NET Core MVC application

**Key Files**:
- `Controllers/HomeController.cs` - Updated with ContactUs actions
- `Views/Home/ContactUs.cshtml` - Contact form view
- `Views/Home/ContactUsSuccess.cshtml` - Success confirmation view
- `Program.cs` - DI configuration
- `appsettings.json` - Configuration settings

---

## Database Setup

### SQL Server Setup

1. **Update Connection String** in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=PrivacyConfirmedDB;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "DatabaseProvider": "SqlServer"
}
```

2. **Run SQL Script**:
   - Open `DatabaseScripts/SqlServer_Setup.sql`
   - Update the database name in the script
   - Execute the script in SQL Server Management Studio

3. **What it creates**:
   - `ContactUs` table with proper indexes
   - `sp_InsertContactUs` stored procedure
   - `sp_GetAllContacts` stored procedure
   - `sp_GetContactById` stored procedure

### PostgreSQL Setup

1. **Update Connection String** in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=privacyconfirmeddb;Username=postgres;Password=your_password;"
  },
  "DatabaseProvider": "PostgreSQL"
}
```

2. **Run SQL Script**:
   - Open `DatabaseScripts/PostgreSQL_Setup.sql`
   - Update the database name if needed
   - Execute the script in pgAdmin or psql

3. **What it creates**:
   - `contactus` table with indexes
   - `sp_insert_contactus` stored procedure
   - `get_all_contacts` function
   - `get_contact_by_id` function
   - `check_duplicate_email` function

---

## Configuration

### appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=PrivacyConfirmedDB;Trusted_Connection=True;TrustServerCertificate=True;",
    "PostgreSQLConnection": "Host=localhost;Database=privacyconfirmeddb;Username=postgres;Password=your_password;"
  },
  "DatabaseProvider": "SqlServer"
}
```

**Configuration Options**:
- `DatabaseProvider`: Set to either `"SqlServer"` or `"PostgreSQL"`
- `DefaultConnection`: Used when `DatabaseProvider` is set

---

## Dependency Injection Setup

In `Program.cs`:

```csharp
// Register Dependency Injection for DAL and BAL
builder.Services.AddScoped<IContactUsRepository, ContactUsRepository>();
builder.Services.AddScoped<IContactUsService, ContactUsService>();
```

**Scope**: `AddScoped` - Creates one instance per HTTP request

---

## API Endpoints

### Contact Us Form
- **URL**: `/Home/ContactUs`
- **Method**: GET
- **Description**: Displays the contact form
- **Returns**: Contact Us view with empty model

### Submit Contact Form
- **URL**: `/Home/ContactUs`
- **Method**: POST
- **Content-Type**: `application/x-www-form-urlencoded`
- **Description**: Processes contact form submission
- **Parameters**:
  - `Name` (string, required, 2-100 chars)
  - `Company` (string, required, 2-150 chars)
  - `MobileNumber` (string, required, 10 digits)
  - `Email` (string, required, valid email)
- **Returns**: 
  - Success: Redirect to ContactUsSuccess
  - Failure: ContactUs view with validation errors

### Success Page
- **URL**: `/Home/ContactUsSuccess`
- **Method**: GET
- **Description**: Displays success confirmation
- **Returns**: Success view

---

## Features Implemented

### 1. Client-Side Validation
- Bootstrap 5 form validation
- Real-time field validation
- Pattern matching for mobile number
- Email format validation
- Required field indicators

### 2. Server-Side Validation
- Model validation using Data Annotations
- Business rule validation (duplicate email)
- Input sanitization
- Error message aggregation

### 3. User Experience
- Modern Bootstrap 5 design
- Responsive layout (mobile-friendly)
- Loading indicators on form submission
- Auto-dismissing alerts
- Icon integration
- Accessibility features (ARIA labels)

### 4. Security Features
- Anti-forgery token validation
- SQL injection prevention (parameterized queries)
- Input sanitization
- XSS protection

### 5. Database Support
- SQL Server (primary)
- PostgreSQL (alternative)
- Runtime database provider selection
- Connection pooling
- Transaction support

---

## Testing

### Test the Feature

1. **Start the Application**:
```bash
dotnet run --project PrivacyConfirmed
```

2. **Navigate to Contact Us**:
   - Go to `https://localhost:5001/Home/ContactUs`
   - Or click "Contact Us" button in the navigation

3. **Fill the Form**:
   - Name: Enter 2-100 characters
   - Company: Enter 2-150 characters
   - Mobile: Enter exactly 10 digits
   - Email: Enter valid email format

4. **Submit and Verify**:
   - Check success page displays
   - Verify database record created
   - Check for duplicate email handling

### Database Verification

**SQL Server**:
```sql
-- View all contacts
SELECT * FROM ContactUs ORDER BY CreatedAt DESC

-- Check specific email
SELECT * FROM ContactUs WHERE Email = 'test@example.com'
```

**PostgreSQL**:
```sql
-- View all contacts
SELECT * FROM contactus ORDER BY createdat DESC;

-- Check specific email
SELECT * FROM contactus WHERE email = 'test@example.com';
```

---

## Code Examples

### Calling the Service from Controller

```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> ContactUs(ContactUsModel model)
{
    if (!ModelState.IsValid)
        return View(model);

    var result = await _contactUsService.SaveContactAsync(model);
    
    if (result.Success)
    {
        TempData["SuccessMessage"] = result.Message;
        return RedirectToAction(nameof(ContactUsSuccess));
    }
    
    TempData["ErrorMessage"] = result.Message;
    return View(model);
}
```

### Using the Repository Directly

```csharp
// In a service or controller
var contact = new ContactUsModel
{
    Name = "John Doe",
    Company = "Tech Corp",
    MobileNumber = "9876543210",
    Email = "john@techcorp.com",
    CreatedAt = DateTime.UtcNow
};

bool success = await _repository.InsertContactAsync(contact);
```

---

## Troubleshooting

### Common Issues

**1. Connection String Error**
```
Error: Cannot open database "PrivacyConfirmedDB"
Solution: Update the connection string in appsettings.json with correct server name
```

**2. Stored Procedure Not Found**
```
Error: Could not find stored procedure 'sp_InsertContactUs'
Solution: Run the appropriate SQL setup script for your database
```

**3. Duplicate Email Error**
```
Error: A contact with this email address already exists
Solution: This is expected behavior - use a different email or delete the existing record
```

**4. Model Validation Fails**
```
Error: Validation failed for one or more fields
Solution: Check that all required fields meet the validation criteria
```

---

## Extension Points

### Adding New Fields

1. **Update Model** (`ContactUsModel.cs`):
```csharp
[StringLength(500)]
public string Message { get; set; } = string.Empty;
```

2. **Update Database**:
```sql
ALTER TABLE ContactUs ADD Message NVARCHAR(500)
```

3. **Update View** (`ContactUs.cshtml`):
```html
<div class="mb-3">
    <label asp-for="Message" class="form-label">Message</label>
    <textarea asp-for="Message" class="form-control" rows="4"></textarea>
    <span asp-validation-for="Message" class="text-danger"></span>
</div>
```

### Adding Email Notifications

Implement in `ContactUsService.SaveContactAsync`:
```csharp
if (success)
{
    await SendEmailNotificationAsync(model);
    // ... existing code
}
```

---

## Performance Considerations

1. **Async/Await**: All database operations are async
2. **Connection Pooling**: Enabled by default in connection strings
3. **Indexes**: Created on Email and CreatedAt columns
4. **Scoped DI**: Proper lifetime management
5. **Validation**: Client-side validation reduces server load

---

## Security Best Practices Implemented

? Anti-forgery tokens on forms
? Parameterized queries (no SQL injection)
? Input sanitization
? Email validation
? HTTPS enforcement
? Model validation
? Error handling (no sensitive data in errors)
? Connection string in configuration (not hardcoded)

---

## Deployment Checklist

- [ ] Update connection strings for production
- [ ] Set DatabaseProvider in production appsettings
- [ ] Run database setup scripts on production database
- [ ] Test database connectivity
- [ ] Enable HTTPS
- [ ] Configure logging
- [ ] Set up email notifications (optional)
- [ ] Test form submission
- [ ] Verify error handling
- [ ] Test with production data

---

## Support and Maintenance

### Logging
Check logs in:
- Console output (Development)
- Application Insights (Production)
- File logs (if configured)

### Database Maintenance
```sql
-- SQL Server: Archive old contacts
SELECT * INTO ContactUs_Archive_2024 
FROM ContactUs 
WHERE CreatedAt < '2024-01-01'

-- PostgreSQL: Archive old contacts
CREATE TABLE contactus_archive_2024 AS 
SELECT * FROM contactus 
WHERE createdat < '2024-01-01'
```

---

## License
This code is part of the PrivacyConfirmed IAM platform.

## Contributors
- Development Team
- Date: 2024

---

## Version History

### v1.0.0 (Current)
- Initial implementation
- SQL Server and PostgreSQL support
- Bootstrap 5 UI
- Clean architecture
- Async operations
- Full validation

---

For questions or support, please contact the development team.
