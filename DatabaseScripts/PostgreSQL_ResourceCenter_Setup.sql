-- =============================================
-- PostgreSQL Database Setup
-- Resource Center Feature
-- =============================================

-- Note: Run this script in your privacyconfirmedwebsite database
-- \c privacyconfirmedwebsite

-- =============================================
-- 1. Create ResourceFiles Table
-- =============================================
CREATE TABLE IF NOT EXISTS resourcefiles (
    id SERIAL PRIMARY KEY,
    filename VARCHAR(255) NOT NULL,
    filepath VARCHAR(500) NOT NULL,
    filesize BIGINT NOT NULL DEFAULT 0,
    fileextension VARCHAR(50) NOT NULL,
    createddate TIMESTAMP NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    isdeleted BOOLEAN NOT NULL DEFAULT FALSE
);

-- Create index on createddate for sorting
CREATE INDEX IF NOT EXISTS idx_resourcefiles_createddate ON resourcefiles(createddate DESC);

-- Create index on isdeleted for filtering
CREATE INDEX IF NOT EXISTS idx_resourcefiles_isdeleted ON resourcefiles(isdeleted);

-- Create index on filename for searching
CREATE INDEX IF NOT EXISTS idx_resourcefiles_filename ON resourcefiles(filename);

-- Add comments to table
COMMENT ON TABLE resourcefiles IS 'Stores metadata for uploaded resource files';

-- Add comments to columns
COMMENT ON COLUMN resourcefiles.id IS 'Unique identifier for resource file record';
COMMENT ON COLUMN resourcefiles.filename IS 'Original name of the uploaded file';
COMMENT ON COLUMN resourcefiles.filepath IS 'Full path where file is stored on server';
COMMENT ON COLUMN resourcefiles.filesize IS 'File size in bytes';
COMMENT ON COLUMN resourcefiles.fileextension IS 'File extension (e.g., .zip, .doc, .xlsx)';
COMMENT ON COLUMN resourcefiles.createddate IS 'Timestamp when the file was uploaded (UTC)';
COMMENT ON COLUMN resourcefiles.isdeleted IS 'Soft delete flag - true if file is deleted';

-- =============================================
-- 2. Create Stored Procedure: sp_insert_resourcefile
-- Purpose: Insert a new resource file record
-- =============================================
CREATE OR REPLACE PROCEDURE sp_insert_resourcefile(
    p_filename VARCHAR(255),
    p_filepath VARCHAR(500),
    p_filesize BIGINT,
    p_fileextension VARCHAR(50),
    p_createddate TIMESTAMP
)
LANGUAGE plpgsql
AS $$
BEGIN
    -- Validate input parameters
    IF p_filename IS NULL OR TRIM(p_filename) = '' THEN
        RAISE EXCEPTION 'File name cannot be empty';
    END IF;
    
    IF p_filepath IS NULL OR TRIM(p_filepath) = '' THEN
        RAISE EXCEPTION 'File path cannot be empty';
    END IF;
    
    IF p_fileextension IS NULL OR TRIM(p_fileextension) = '' THEN
        RAISE EXCEPTION 'File extension cannot be empty';
    END IF;
    
    IF p_filesize < 0 THEN
        RAISE EXCEPTION 'File size cannot be negative';
    END IF;
    
    -- Insert the record
    INSERT INTO resourcefiles (
        filename,
        filepath,
        filesize,
        fileextension,
        createddate,
        isdeleted
    )
    VALUES (
        TRIM(p_filename),
        TRIM(p_filepath),
        p_filesize,
        LOWER(TRIM(p_fileextension)),
        p_createddate,
        FALSE
    );
    
    RAISE NOTICE 'Resource file record inserted successfully';
    
EXCEPTION
    WHEN OTHERS THEN
        RAISE EXCEPTION 'Error inserting resource file: %', SQLERRM;
END;
$$;

-- Add comment to procedure
COMMENT ON PROCEDURE sp_insert_resourcefile IS 'Inserts a new resource file record with validation';

-- =============================================
-- 3. Create Function: get_all_resourcefiles
-- Purpose: Retrieve all non-deleted resource files
-- =============================================
CREATE OR REPLACE FUNCTION get_all_resourcefiles()
RETURNS TABLE (
    id INT,
    filename VARCHAR(255),
    filepath VARCHAR(500),
    filesize BIGINT,
    fileextension VARCHAR(50),
    createddate TIMESTAMP,
    isdeleted BOOLEAN
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT 
        rf.id,
        rf.filename,
        rf.filepath,
        rf.filesize,
        rf.fileextension,
        rf.createddate,
        rf.isdeleted
    FROM resourcefiles rf
    WHERE rf.isdeleted = FALSE
    ORDER BY rf.createddate DESC;
END;
$$;

-- Add comment to function
COMMENT ON FUNCTION get_all_resourcefiles IS 'Retrieves all non-deleted resource files ordered by creation date descending';

-- =============================================
-- 4. Create Function: get_resourcefile_by_id
-- Purpose: Retrieve a single resource file by ID
-- =============================================
CREATE OR REPLACE FUNCTION get_resourcefile_by_id(p_id INT)
RETURNS TABLE (
    id INT,
    filename VARCHAR(255),
    filepath VARCHAR(500),
    filesize BIGINT,
    fileextension VARCHAR(50),
    createddate TIMESTAMP,
    isdeleted BOOLEAN
)
LANGUAGE plpgsql
AS $$
BEGIN
    IF p_id IS NULL OR p_id <= 0 THEN
        RAISE EXCEPTION 'Invalid resource file ID';
    END IF;
    
    RETURN QUERY
    SELECT 
        rf.id,
        rf.filename,
        rf.filepath,
        rf.filesize,
        rf.fileextension,
        rf.createddate,
        rf.isdeleted
    FROM resourcefiles rf
    WHERE rf.id = p_id;
END;
$$;

-- Add comment to function
COMMENT ON FUNCTION get_resourcefile_by_id IS 'Retrieves a single resource file record by ID';

-- =============================================
-- 5. Create Procedure: sp_delete_resourcefile
-- Purpose: Soft delete a resource file by setting isdeleted = true
-- =============================================
CREATE OR REPLACE PROCEDURE sp_delete_resourcefile(p_id INT)
LANGUAGE plpgsql
AS $$
BEGIN
    IF p_id IS NULL OR p_id <= 0 THEN
        RAISE EXCEPTION 'Invalid resource file ID';
    END IF;
    
    -- Check if record exists
    IF NOT EXISTS (SELECT 1 FROM resourcefiles WHERE id = p_id) THEN
        RAISE EXCEPTION 'Resource file with ID % not found', p_id;
    END IF;
    
    -- Soft delete by setting isdeleted = true
    UPDATE resourcefiles
    SET isdeleted = TRUE
    WHERE id = p_id;
    
    RAISE NOTICE 'Resource file with ID % deleted successfully', p_id;
    
EXCEPTION
    WHEN OTHERS THEN
        RAISE EXCEPTION 'Error deleting resource file: %', SQLERRM;
END;
$$;

-- Add comment to procedure
COMMENT ON PROCEDURE sp_delete_resourcefile IS 'Soft deletes a resource file by setting isdeleted flag to true';

-- =============================================
-- Test Data (Optional - for development/testing)
-- =============================================
/*
-- Insert test data
CALL sp_insert_resourcefile(
    'Sample_Document.docx',
    '/UploadedFiles/Sample_Document_20240115.docx',
    15360,
    '.docx',
    NOW() AT TIME ZONE 'UTC'
);

CALL sp_insert_resourcefile(
    'Project_Files.zip',
    '/UploadedFiles/Project_Files_20240115.zip',
    2048000,
    '.zip',
    NOW() AT TIME ZONE 'UTC'
);

-- Retrieve all resource files
SELECT * FROM get_all_resourcefiles();

-- Retrieve specific resource file
SELECT * FROM get_resourcefile_by_id(1);

-- Delete a resource file
CALL sp_delete_resourcefile(1);

-- Verify deletion
SELECT * FROM get_all_resourcefiles();
*/

-- =============================================
-- Grant Permissions (adjust as needed for your setup)
-- =============================================
-- GRANT SELECT, INSERT, UPDATE ON resourcefiles TO webuser;
-- GRANT EXECUTE ON PROCEDURE sp_insert_resourcefile TO webuser;
-- GRANT EXECUTE ON PROCEDURE sp_delete_resourcefile TO webuser;
-- GRANT EXECUTE ON FUNCTION get_all_resourcefiles TO webuser;
-- GRANT EXECUTE ON FUNCTION get_resourcefile_by_id TO webuser;
-- GRANT USAGE, SELECT ON SEQUENCE resourcefiles_id_seq TO webuser;

-- Display success message
DO $$
BEGIN
    RAISE NOTICE 'PostgreSQL Resource Center setup completed successfully!';
END $$;
