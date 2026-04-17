using PrivacyConfirmedModel;
using PrivacyConfirmedDAL.Interfaces;
using System.Data;
using Npgsql;
using NpgsqlTypes;

namespace PrivacyConfirmedDAL.Repositories
{
    #region Resource File Repository
    /// <summary>
    /// Repository class for Resource File data access
    /// Supports PostgreSQL database
    /// </summary>
    public class ResourceFileRepository : IResourceFileRepository
    {
        #region Private Fields
        private readonly string _connectionString;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the ResourceFileRepository
        /// </summary>
        /// <param name="connectionString">PostgreSQL connection string</param>
        public ResourceFileRepository(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Inserts a new resource file record into the database using stored procedure
        /// </summary>
        /// <param name="model">Resource file model with file information</param>
        /// <returns>True if insert was successful, false otherwise</returns>
        public async Task<bool> InsertResourceFileAsync(ResourceFileModel model)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                var commandText = "CALL sp_insert_resourcefile(@p_filename, @p_filepath, @p_filesize, @p_fileextension, @p_createddate)";

                using var command = new NpgsqlCommand(commandText, connection)
                {
                    CommandType = CommandType.Text
                };

                // Add parameters with explicit types
                command.Parameters.Add(new NpgsqlParameter("p_filename", NpgsqlDbType.Varchar) { Value = model.FileName });
                command.Parameters.Add(new NpgsqlParameter("p_filepath", NpgsqlDbType.Varchar) { Value = model.FilePath });
                command.Parameters.Add(new NpgsqlParameter("p_filesize", NpgsqlDbType.Bigint) { Value = model.FileSize });
                command.Parameters.Add(new NpgsqlParameter("p_fileextension", NpgsqlDbType.Varchar) { Value = model.FileExtension });

                // Convert to Unspecified DateTime for timestamp without time zone
                var createdDateWithoutTz = DateTime.SpecifyKind(model.CreatedDate, DateTimeKind.Unspecified);
                command.Parameters.Add(new NpgsqlParameter("p_createddate", NpgsqlDbType.Timestamp) { Value = createdDateWithoutTz });

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting resource file: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Gets all non-deleted resource files from the database
        /// </summary>
        /// <returns>List of all active resource files</returns>
        public async Task<List<ResourceFileModel>> GetAllResourceFilesAsync()
        {
            var resourceFiles = new List<ResourceFileModel>();

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(
                    "SELECT id, filename, filepath, filesize, fileextension, createddate, isdeleted FROM resourcefiles WHERE isdeleted = FALSE ORDER BY createddate DESC", 
                    connection);

                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();
                
                while (await reader.ReadAsync())
                {
                    resourceFiles.Add(MapResourceFileFromReader(reader));
                }

                return resourceFiles;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving resource files: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Gets a resource file record by ID
        /// </summary>
        /// <param name="id">Resource file ID</param>
        /// <returns>Resource file record or null if not found</returns>
        public async Task<ResourceFileModel?> GetResourceFileByIdAsync(int id)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(
                    "SELECT id, filename, filepath, filesize, fileextension, createddate, isdeleted FROM resourcefiles WHERE id = @id", 
                    connection);

                command.Parameters.AddWithValue("@id", id);

                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();
                
                if (await reader.ReadAsync())
                {
                    return MapResourceFileFromReader(reader);
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving resource file by ID: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Soft deletes a resource file by setting isdeleted = true
        /// </summary>
        /// <param name="id">Resource file ID</param>
        /// <returns>True if delete was successful, false otherwise</returns>
        public async Task<bool> DeleteResourceFileAsync(int id)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                var commandText = "CALL sp_delete_resourcefile(@p_id)";

                using var command = new NpgsqlCommand(commandText, connection)
                {
                    CommandType = CommandType.Text
                };

                command.Parameters.Add(new NpgsqlParameter("p_id", NpgsqlDbType.Integer) { Value = id });

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting resource file: {ex.Message}");
                throw;
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Maps data reader to ResourceFileModel
        /// </summary>
        private ResourceFileModel MapResourceFileFromReader(IDataReader reader)
        {
            return new ResourceFileModel
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                FileName = reader.GetString(reader.GetOrdinal("filename")),
                FilePath = reader.GetString(reader.GetOrdinal("filepath")),
                FileSize = reader.GetInt64(reader.GetOrdinal("filesize")),
                FileExtension = reader.GetString(reader.GetOrdinal("fileextension")),
                CreatedDate = reader.GetDateTime(reader.GetOrdinal("createddate")),
                IsDeleted = reader.GetBoolean(reader.GetOrdinal("isdeleted"))
            };
        }

        #endregion
    }
    #endregion
}
