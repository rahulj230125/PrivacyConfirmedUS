-- =============================================
-- SQL Server Database Setup
-- Contact Us Feature
-- =============================================

USE [YourDatabaseName]
GO

-- =============================================
-- 1. Create ContactUs Table
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ContactUs]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ContactUs](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [Name] [nvarchar](100) NOT NULL,
        [Company] [nvarchar](150) NOT NULL,
        [MobileNumber] [nvarchar](10) NOT NULL,
        [Email] [nvarchar](150) NOT NULL,
        [CreatedAt] [datetime] NOT NULL DEFAULT (GETUTCDATE()),
        CONSTRAINT [PK_ContactUs] PRIMARY KEY CLUSTERED ([Id] ASC)
    )
    
    -- Create index on Email for better performance on duplicate checks
    CREATE NONCLUSTERED INDEX [IX_ContactUs_Email] ON [dbo].[ContactUs]
    (
        [Email] ASC
    )
    
    -- Create index on CreatedAt for sorting
    CREATE NONCLUSTERED INDEX [IX_ContactUs_CreatedAt] ON [dbo].[ContactUs]
    (
        [CreatedAt] DESC
    )
    
    PRINT 'ContactUs table created successfully'
END
ELSE
BEGIN
    PRINT 'ContactUs table already exists'
END
GO

-- =============================================
-- 2. Create Stored Procedure: sp_InsertContactUs
-- Purpose: Insert a new contact record
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_InsertContactUs]') AND type in (N'P', N'PC'))
BEGIN
    DROP PROCEDURE [dbo].[sp_InsertContactUs]
    PRINT 'Existing sp_InsertContactUs procedure dropped'
END
GO

CREATE PROCEDURE [dbo].[sp_InsertContactUs]
    @Name NVARCHAR(100),
    @Company NVARCHAR(150),
    @MobileNumber NVARCHAR(10),
    @Email NVARCHAR(150),
    @CreatedAt DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION
        
        -- Validate input parameters
        IF @Name IS NULL OR LTRIM(RTRIM(@Name)) = ''
        BEGIN
            RAISERROR('Name cannot be empty', 16, 1)
            RETURN
        END
        
        IF @Company IS NULL OR LTRIM(RTRIM(@Company)) = ''
        BEGIN
            RAISERROR('Company cannot be empty', 16, 1)
            RETURN
        END
        
        IF @MobileNumber IS NULL OR LTRIM(RTRIM(@MobileNumber)) = ''
        BEGIN
            RAISERROR('Mobile number cannot be empty', 16, 1)
            RETURN
        END
        
        IF @Email IS NULL OR LTRIM(RTRIM(@Email)) = ''
        BEGIN
            RAISERROR('Email cannot be empty', 16, 1)
            RETURN
        END
        
        -- Insert the record
        INSERT INTO [dbo].[ContactUs]
        (
            [Name],
            [Company],
            [MobileNumber],
            [Email],
            [CreatedAt]
        )
        VALUES
        (
            @Name,
            @Company,
            @MobileNumber,
            @Email,
            @CreatedAt
        )
        
        COMMIT TRANSACTION
        
        PRINT 'Contact record inserted successfully'
        
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION
        
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE()
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY()
        DECLARE @ErrorState INT = ERROR_STATE()
        
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState)
    END CATCH
END
GO

PRINT 'sp_InsertContactUs procedure created successfully'
GO

-- =============================================
-- 3. Create Stored Procedure: sp_GetAllContacts
-- Purpose: Retrieve all contact records
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetAllContacts]') AND type in (N'P', N'PC'))
BEGIN
    DROP PROCEDURE [dbo].[sp_GetAllContacts]
END
GO

CREATE PROCEDURE [dbo].[sp_GetAllContacts]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        [Id],
        [Name],
        [Company],
        [MobileNumber],
        [Email],
        [CreatedAt]
    FROM [dbo].[ContactUs]
    ORDER BY [CreatedAt] DESC
END
GO

PRINT 'sp_GetAllContacts procedure created successfully'
GO

-- =============================================
-- 4. Create Stored Procedure: sp_GetContactById
-- Purpose: Retrieve a single contact by ID
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetContactById]') AND type in (N'P', N'PC'))
BEGIN
    DROP PROCEDURE [dbo].[sp_GetContactById]
END
GO

CREATE PROCEDURE [dbo].[sp_GetContactById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        [Id],
        [Name],
        [Company],
        [MobileNumber],
        [Email],
        [CreatedAt]
    FROM [dbo].[ContactUs]
    WHERE [Id] = @Id
END
GO

PRINT 'sp_GetContactById procedure created successfully'
GO

-- =============================================
-- Test Data (Optional - for development/testing)
-- =============================================
/*
-- Insert test data
EXEC [dbo].[sp_InsertContactUs]
    @Name = 'John Doe',
    @Company = 'Tech Solutions Inc',
    @MobileNumber = '9876543210',
    @Email = 'john.doe@techsolutions.com',
    @CreatedAt = '2024-01-15 10:30:00'

-- Retrieve all contacts
EXEC [dbo].[sp_GetAllContacts]

-- Retrieve specific contact
EXEC [dbo].[sp_GetContactById] @Id = 1
*/

PRINT 'SQL Server setup completed successfully!'
GO
