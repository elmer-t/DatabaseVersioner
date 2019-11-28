using System;
using System.Data.SqlClient;

namespace DatabaseVersioner
{

    /// <summary>
    /// Connects to a specified database / catalog
    /// </summary>
    public class DataSource : IDisposable
    {

        /// <summary>
        /// Gets or sets the connection string
        /// </summary>
        private string ConnectionString { get; set; }

        public string DatabaseName { get; private set; }
        public string DataSourceName { get; private set; }
        public string WorkStationId { get; private set; }
        public string ServerVersion { get; private set; }
        public string TableName { get; private set; }
        public int? Timeout { get; private set; }

        public DataSource(Arguments settings)
        {
            ConnectionString = settings.ConnectionString;
            TableName = settings.TableName;
            Timeout = settings.DatabaseTimeout;

            using (SqlConnection conn = new SqlConnection(ConnectionString)) {

                conn.Open();

                DatabaseName = conn.Database;
                DataSourceName = conn.DataSource;
                WorkStationId = conn.WorkstationId;
                ServerVersion = conn.ServerVersion;

            }
        }

        public bool SchemaChangesTableExists() {

            bool result = false;

            using (SqlConnection conn = new SqlConnection(ConnectionString)) {

                conn.Open();

                using (SqlCommand cmd = new SqlCommand(string.Format("SELECT TABLE_NAME FROM [INFORMATION_SCHEMA].[TABLES] WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='{0}'", this.TableName), conn)) {
                    var reader = cmd.ExecuteReader();

                    if (reader != null) {
                        while (reader.Read()) {
                            result = !String.IsNullOrWhiteSpace(reader.GetString(0));
                        }
                    }
                }

                conn.Close();
            }
            return result;

        }

        public void CreateSchemaChangesTable() {

            string sql = $@"
                CREATE TABLE [dbo].[{TableName}](
                [ID] [int] IDENTITY(1,1) NOT NULL,
                [Major] [int] NOT NULL,
                [Minor] [int] NOT NULL,
                [Revision] [int] NOT NULL,
                [Build] [int] NOT NULL,
                [Description] [varchar](255) NOT NULL,
                [DateApplied] [datetime] NOT NULL,

	            CONSTRAINT [PK_{TableName}] 
		            PRIMARY KEY CLUSTERED ([ID] ASC)
            )";

            using (SqlConnection conn = new SqlConnection(ConnectionString)) {

                conn.Open();

                using (SqlCommand cmd = new SqlCommand(sql, conn)) {
                    var reader = cmd.ExecuteNonQuery();
                }

                conn.Close();
            }
        }
        
        /// <summary>
        /// Gets the current schema version of the connected database
        /// </summary>
        public SchemaVersion GetSchemaVersion()
        {

            SchemaVersion version = new SchemaVersion();
            using (SqlConnection conn = new SqlConnection(ConnectionString)) {

                conn.Open();

                using (SqlCommand cmd = new SqlCommand(string.Format("SELECT TOP 1 Major, Minor, Revision, Build FROM [{0}] ORDER BY Major DESC, Minor DESC, Revision DESC, Build DESC", this.TableName), conn)) {
                    var reader = cmd.ExecuteReader();

                    if (reader != null) {
                        while (reader.Read()) {
                            version = new SchemaVersion(reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2), reader.GetInt32(3));
                        }
                    }
                }

                conn.Close();
            }
            return version;
        }

        /// <summary>
        /// Executes the specified script on the connected database
        /// </summary>
        /// <param name="script"></param>
        /// <param name="version"></param>
        /// <param name="description"></param>
        /// <returns>True if succeeded, False is errors occured</returns>
        public void ExecuteScript(string script, SchemaVersion version, string description)
        {

            using (SqlConnection conn = new SqlConnection(ConnectionString)) {

                conn.Open();

                var expectedVersion = GetSchemaVersion();

                // Don't perform version check if schema version is not initialized (ie. database is new)
                if (expectedVersion.IsInitialized) {
                    expectedVersion.Build++;

                    if (version != expectedVersion) {
                        if (IsScriptProcessed(version)) {
                            throw new DuplicateScriptException(version);
                        } else {
                            throw new InvalidSchemaVersionException(version, expectedVersion);
                        }
                    }
                }

                using (var trans = new System.Transactions.TransactionScope()) {

                    using (SqlCommand cmd = new SqlCommand(script, conn))
                    {
                        if (Timeout.HasValue) cmd.CommandTimeout = Timeout.Value;
                        var x = cmd.ExecuteNonQuery();

                        script = string.Format(
                             "INSERT INTO [{0}] (Major, Minor, Revision, Build, Description, DateApplied) VALUES (@Major, @Minor, @Revision, @Build, @Description, GETDATE())",
                             TableName
                        );

                        cmd.CommandText = script;
                        cmd.Parameters.AddWithValue("@Major", version.Major);
                        cmd.Parameters.AddWithValue("@Minor", version.Minor);
                        cmd.Parameters.AddWithValue("@Revision", version.Revision);
                        cmd.Parameters.AddWithValue("@Build", version.Build);
                        cmd.Parameters.AddWithValue("@Description", description);

                        var y = cmd.ExecuteNonQuery();
                    }

                    trans.Complete();
                }
            }
        }

        /// <summary>
        /// Checks if a specified script version has been processed.
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        private bool IsScriptProcessed(SchemaVersion version)
        {
            bool output;

            using (SqlConnection conn = new SqlConnection(ConnectionString)) {

                conn.Open();

                var script =
                    string.Format(
                        "SELECT COUNT(*) FROM [{0}] WHERE Major=@Major AND Minor=@Minor AND Revision=@Revision AND Build=@Build",
                        TableName);

                using (SqlCommand cmd = new SqlCommand(script, conn)) {

                    cmd.Parameters.AddWithValue("@Major", version.Major);
                    cmd.Parameters.AddWithValue("@Minor", version.Minor);
                    cmd.Parameters.AddWithValue("@Revision", version.Revision);
                    cmd.Parameters.AddWithValue("@Build", version.Build);

                    var count = Convert.ToInt32(cmd.ExecuteScalar());
                    output = count > 0;
                }

                conn.Close();
            }
            return output;
        }

        public void Dispose()
        {
            // TODO: dispose db connections
        }
    }
}
