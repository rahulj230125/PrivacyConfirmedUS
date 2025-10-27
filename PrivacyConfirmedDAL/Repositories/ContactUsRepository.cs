using Microsoft.Extensions.Configuration;
using PrivacyConfirmedModel;
using PrivacyConfirmedDAL.Interfaces;
using System.Data;
using System.Data.SqlClient;
using Npgsql;

namespace PrivacyConfirmedDAL.Repositories
{
    #region Contact Us Repository
    /// <summary>
    /// Repository class for Contact Us data access
    /// Supports both SQL Server and PostgreSQL databases
    /// </summary>
    public class ContactUsRepository : IContactUsRepository
    {
        #region Private Fields
        private readonly string _connectionString;
        private readonly string _databaseProvider;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the ContactUsRepository
        /// </summary>
        /// <param name="configuration">Configuration service for accessing app settings</param>
        public ContactUsRepository(IConfiguration configuration)
        {
            _databaseProvider = configuration["DatabaseProvider"] ?? "SqlServer";

            if (_databaseProvider.Equals("PostgreSQL", StringComparison.OrdinalIgnoreCase))
            {
                _connectionString = configuration.GetConnectionString("PostgreSQLConnection")
                    ?? throw new ArgumentNullException(nameof(configuration), "PostgreSQLConnection is missing in configuration.");
                // Quick sanity check
                if (_connectionString.Contains("Trusted_Connection", StringComparison.OrdinalIgnoreCase) ||
                    _connectionString.Contains("Integrated Security", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException("PostgreSQLConnection contains SQL Server keywords (Trusted_Connection / Integrated Security). Use a PostgreSQL connection string like: \"Host=localhost;Database=privacyconfirmeddb;Username=postgres;Password=your_password;\"");
                }
            }
            else
            {
                _connectionString = configuration.GetConnectionString("DefaultConnection")
                    ?? throw new ArgumentNullException(nameof(configuration), "DefaultConnection is missing in configuration.");
            }
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Inserts a new contact record into the database using stored procedure
        /// </summary>
        /// <param name="model">Contact Us model with user information</param>
        /// <returns>True if insert was successful, false otherwise</returns>
        public async Task<bool> InsertContactAsync(ContactUsModel model)
        {
            try
            {
                if (_databaseProvider.Equals("PostgreSQL", StringComparison.OrdinalIgnoreCase))
                {
                    return await InsertContactPostgreSQLAsync(model);
                }
                else
                {
                    return await InsertContactSqlServerAsync(model);
                }
            }
            catch (Exception ex)
            {
                // Log the exception (in a real application, use proper logging)
                Console.WriteLine($"Error inserting contact: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Gets all contact records from the database
        /// </summary>
        /// <returns>List of all contact records</returns>
        public async Task<List<ContactUsModel>> GetAllContactsAsync()
        {
            var contacts = new List<ContactUsModel>();

            try
            {
                if (_databaseProvider.Equals("PostgreSQL", StringComparison.OrdinalIgnoreCase))
                {
                    return await GetAllContactsPostgreSQLAsync();
                }
                else
                {
                    return await GetAllContactsSqlServerAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving contacts: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Gets a contact record by ID
        /// </summary>
        /// <param name="id">Contact ID</param>
        /// <returns>Contact record or null if not found</returns>
        public async Task<ContactUsModel?> GetContactByIdAsync(int id)
        {
            try
            {
                if (_databaseProvider.Equals("PostgreSQL", StringComparison.OrdinalIgnoreCase))
                {
                    return await GetContactByIdPostgreSQLAsync(id);
                }
                else
                {
                    return await GetContactByIdSqlServerAsync(id);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving contact by ID: {ex.Message}");
                throw;
            }
        }

        #endregion

        #region SQL Server Methods

        /// <summary>
        /// Inserts contact using SQL Server stored procedure
        /// </summary>
        private async Task<bool> InsertContactSqlServerAsync(ContactUsModel model)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_InsertContactUs", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Add parameters
                    command.Parameters.AddWithValue("@Name", model.Name);
                    command.Parameters.AddWithValue("@Company", model.Company);
                    command.Parameters.AddWithValue("@MobileNumber", model.MobileNumber);
                    command.Parameters.AddWithValue("@Email", model.Email);
                    command.Parameters.AddWithValue("@CreatedAt", model.CreatedAt);

                    await connection.OpenAsync();
                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }

        /// <summary>
        /// Gets all contacts using SQL Server
        /// </summary>
        private async Task<List<ContactUsModel>> GetAllContactsSqlServerAsync()
        {
            var contacts = new List<ContactUsModel>();

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("SELECT Id, Name, Company, MobileNumber, Email, CreatedAt FROM ContactUs ORDER BY CreatedAt DESC", connection))
                {
                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            contacts.Add(MapContactFromReader(reader));
                        }
                    }
                }
            }

            return contacts;
        }

        /// <summary>
        /// Gets contact by ID using SQL Server
        /// </summary>
        private async Task<ContactUsModel?> GetContactByIdSqlServerAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("SELECT Id, Name, Company, MobileNumber, Email, CreatedAt FROM ContactUs WHERE Id = @Id", connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return MapContactFromReader(reader);
                        }
                    }
                }
            }

            return null;
        }

        #endregion

        #region PostgreSQL Methods

        /// <summary>
        /// Inserts contact using PostgreSQL stored procedure
        /// </summary>
        private async Task<bool> InsertContactPostgreSQLAsync(ContactUsModel model)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            // fully qualified name can be used if procedure is in a non-default schema:
            // var commandText = "CALL public.sp_insert_contactus(@p_name, @p_company, @p_mobilenumber, @p_email, @p_createdat)";
            var commandText = "CALL sp_insert_contactus(@p_name, @p_company, @p_mobilenumber, @p_email, @p_createdat)";

            using var command = new NpgsqlCommand(commandText, connection)
            {
                CommandType = CommandType.Text
            };

            // String parameters
            command.Parameters.Add(new NpgsqlParameter("p_name", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = model.Name ?? string.Empty });
            command.Parameters.Add(new NpgsqlParameter("p_company", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = model.Company ?? string.Empty });
            command.Parameters.Add(new NpgsqlParameter("p_mobilenumber", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = model.MobileNumber ?? string.Empty });
            command.Parameters.Add(new NpgsqlParameter("p_email", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = (model.Email ?? string.Empty).Trim().ToLowerInvariant() });

            // Ensure the timestamp parameter matches the procedure's TIMESTAMP (without time zone) type.
            // Convert to an Unspecified DateTime so Npgsql sends it as timestamp (no time zone).
            var createdAtWithoutTz = DateTime.SpecifyKind(model.CreatedAt, DateTimeKind.Unspecified);
            command.Parameters.Add(new NpgsqlParameter("p_createdat", NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = createdAtWithoutTz });

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
            return true;
        }

        /// <summary>
        /// Gets all contacts using PostgreSQL
        /// </summary>
        private async Task<List<ContactUsModel>> GetAllContactsPostgreSQLAsync()
        {
            var contacts = new List<ContactUsModel>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                using (var command = new NpgsqlCommand("SELECT id, name, company, mobilenumber, email, createdat FROM contactus ORDER BY createdat DESC", connection))
                {
                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            contacts.Add(MapContactFromReaderPostgreSQL(reader));
                        }
                    }
                }
            }

            return contacts;
        }

        /// <summary>
        /// Gets contact by ID using PostgreSQL
        /// </summary>
        private async Task<ContactUsModel?> GetContactByIdPostgreSQLAsync(int id)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                using (var command = new NpgsqlCommand("SELECT id, name, company, mobilenumber, email, createdat FROM contactus WHERE id = @id", connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return MapContactFromReaderPostgreSQL(reader);
                        }
                    }
                }
            }

            return null;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Maps data reader to ContactUsModel for SQL Server
        /// </summary>
        private ContactUsModel MapContactFromReader(IDataReader reader)
        {
            return new ContactUsModel
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                Company = reader.GetString(reader.GetOrdinal("Company")),
                MobileNumber = reader.GetString(reader.GetOrdinal("MobileNumber")),
                Email = reader.GetString(reader.GetOrdinal("Email")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
            };
        }

        /// <summary>
        /// Maps data reader to ContactUsModel for PostgreSQL
        /// </summary>
        private ContactUsModel MapContactFromReaderPostgreSQL(IDataReader reader)
        {
            return new ContactUsModel
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                Name = reader.GetString(reader.GetOrdinal("name")),
                Company = reader.GetString(reader.GetOrdinal("company")),
                MobileNumber = reader.GetString(reader.GetOrdinal("mobilenumber")),
                Email = reader.GetString(reader.GetOrdinal("email")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("createdat"))
            };
        }

        #endregion
    }
    #endregion
}
