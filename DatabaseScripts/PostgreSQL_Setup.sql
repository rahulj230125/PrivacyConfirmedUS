-- =============================================
-- PostgreSQL Database Setup
-- Contact Us Feature

CREATE DATABASE PrivacyConfirmedWebsite;
CREATE USER webuser WITH PASSWORD 'P@ss55word';
GRANT ALL PRIVILEGES ON DATABASE PrivacyConfirmedWebsite TO webuser;


-- =============================================

-- Note: Replace 'your_database_name' with your actual database name
-- \c your_database_name

-- =============================================
-- 1. Create ContactUs Table
-- =============================================
CREATE TABLE IF NOT EXISTS contactus (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    company VARCHAR(150) NOT NULL,
    mobilenumber VARCHAR(10) NOT NULL,
    email VARCHAR(150) NOT NULL,
    createdat TIMESTAMP NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC')
);

-- Create index on email for better performance on duplicate checks
CREATE INDEX IF NOT EXISTS idx_contactus_email ON contactus(email);

-- Create index on createdat for sorting
CREATE INDEX IF NOT EXISTS idx_contactus_createdat ON contactus(createdat DESC);

-- Add comment to table
COMMENT ON TABLE contactus IS 'Stores contact information from Contact Us form submissions';

-- Add comments to columns
COMMENT ON COLUMN contactus.id IS 'Unique identifier for contact record';
COMMENT ON COLUMN contactus.name IS 'Full name of the person contacting';
COMMENT ON COLUMN contactus.company IS 'Company name';
COMMENT ON COLUMN contactus.mobilenumber IS '10-digit mobile number';
COMMENT ON COLUMN contactus.email IS 'Email address of the contact';
COMMENT ON COLUMN contactus.createdat IS 'Timestamp when the record was created (UTC)';

-- =============================================
-- 2. Create Stored Procedure: sp_insert_contactus
-- Purpose: Insert a new contact record
-- =============================================
CREATE OR REPLACE PROCEDURE sp_insert_contactus(
    p_name VARCHAR(100),
    p_company VARCHAR(150),
    p_mobilenumber VARCHAR(10),
    p_email VARCHAR(150),
    p_createdat TIMESTAMP
)
LANGUAGE plpgsql
AS $$
BEGIN
    -- Validate input parameters
    IF p_name IS NULL OR TRIM(p_name) = '' THEN
        RAISE EXCEPTION 'Name cannot be empty';
    END IF;
    
    IF p_company IS NULL OR TRIM(p_company) = '' THEN
        RAISE EXCEPTION 'Company cannot be empty';
    END IF;
    
    IF p_mobilenumber IS NULL OR TRIM(p_mobilenumber) = '' THEN
        RAISE EXCEPTION 'Mobile number cannot be empty';
    END IF;
    
    IF p_email IS NULL OR TRIM(p_email) = '' THEN
        RAISE EXCEPTION 'Email cannot be empty';
    END IF;
    
    -- Validate mobile number format (10 digits)
    IF p_mobilenumber !~ '^[0-9]{10}$' THEN
        RAISE EXCEPTION 'Mobile number must be a valid 10-digit number';
    END IF;
    
    -- Validate email format (basic validation)
    IF p_email !~ '^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$' THEN
        RAISE EXCEPTION 'Email address is not valid';
    END IF;
    
    -- Insert the record
    INSERT INTO contactus (
        name,
        company,
        mobilenumber,
        email,
        createdat
    )
    VALUES (
        TRIM(p_name),
        TRIM(p_company),
        TRIM(p_mobilenumber),
        LOWER(TRIM(p_email)),
        p_createdat
    );
    
    RAISE NOTICE 'Contact record inserted successfully';
    
EXCEPTION
    WHEN OTHERS THEN
        RAISE EXCEPTION 'Error inserting contact: %', SQLERRM;
END;
$$;

-- Add comment to procedure
COMMENT ON PROCEDURE sp_insert_contactus IS 'Inserts a new contact record into the contactus table with validation';

-- =============================================
-- 3. Create Function: get_all_contacts
-- Purpose: Retrieve all contact records
-- =============================================
CREATE OR REPLACE FUNCTION get_all_contacts()
RETURNS TABLE (
    id INT,
    name VARCHAR(100),
    company VARCHAR(150),
    mobilenumber VARCHAR(10),
    email VARCHAR(150),
    createdat TIMESTAMP
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT 
        c.id,
        c.name,
        c.company,
        c.mobilenumber,
        c.email,
        c.createdat
    FROM contactus c
    ORDER BY c.createdat DESC;
END;
$$;

-- Add comment to function
COMMENT ON FUNCTION get_all_contacts IS 'Retrieves all contact records ordered by creation date descending';

-- =============================================
-- 4. Create Function: get_contact_by_id
-- Purpose: Retrieve a single contact by ID
-- =============================================
CREATE OR REPLACE FUNCTION get_contact_by_id(p_id INT)
RETURNS TABLE (
    id INT,
    name VARCHAR(100),
    company VARCHAR(150),
    mobilenumber VARCHAR(10),
    email VARCHAR(150),
    createdat TIMESTAMP
)
LANGUAGE plpgsql
AS $$
BEGIN
    IF p_id IS NULL OR p_id <= 0 THEN
        RAISE EXCEPTION 'Invalid contact ID';
    END IF;
    
    RETURN QUERY
    SELECT 
        c.id,
        c.name,
        c.company,
        c.mobilenumber,
        c.email,
        c.createdat
    FROM contactus c
    WHERE c.id = p_id;
END;
$$;

-- Add comment to function
COMMENT ON FUNCTION get_contact_by_id IS 'Retrieves a single contact record by ID';

-- =============================================
-- 5. Create Function: check_duplicate_email
-- Purpose: Check if email already exists
-- =============================================
CREATE OR REPLACE FUNCTION check_duplicate_email(p_email VARCHAR(150))
RETURNS BOOLEAN
LANGUAGE plpgsql
AS $$
DECLARE
    email_exists BOOLEAN;
BEGIN
    SELECT EXISTS(
        SELECT 1 
        FROM contactus 
        WHERE LOWER(email) = LOWER(TRIM(p_email))
    ) INTO email_exists;
    
    RETURN email_exists;
END;
$$;

-- Add comment to function
COMMENT ON FUNCTION check_duplicate_email IS 'Checks if an email address already exists in the contactus table';

-- =============================================
-- Test Data (Optional - for development/testing)
-- =============================================
/*
-- Insert test data
CALL sp_insert_contactus(
    'John Doe',
    'Tech Solutions Inc',
    '9876543210',
    'john.doe@techsolutions.com',
    NOW() AT TIME ZONE 'UTC'
);

-- Retrieve all contacts
SELECT * FROM get_all_contacts();

-- Retrieve specific contact
SELECT * FROM get_contact_by_id(1);

-- Check for duplicate email
SELECT check_duplicate_email('john.doe@techsolutions.com');
*/

-- =============================================
-- Grant Permissions (adjust as needed for your setup)
-- =============================================
GRANT SELECT, INSERT ON contactus TO webuser;
GRANT EXECUTE ON PROCEDURE sp_insert_contactus TO webuser;
GRANT EXECUTE ON FUNCTION get_all_contacts TO webuser;
GRANT EXECUTE ON FUNCTION get_contact_by_id TO webuser;
GRANT EXECUTE ON FUNCTION check_duplicate_email TO webuser;

-- Display success message
DO $$
BEGIN
    RAISE NOTICE 'PostgreSQL setup completed successfully!';
END $$;
