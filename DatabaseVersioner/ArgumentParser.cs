using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseVersioner {

    public class ArgumentParser {

        /// <summary>
        /// Parse incoming arguments
        /// </summary>
        /// <param name="args"></param>
        public static Arguments Parse(string[] args)
        {

            var settings = new Arguments();

            foreach (string arg in args) {

                if (arg.IndexOf(":") > -1) {

                    string key = arg.Substring(0, arg.IndexOf(':'));
                    string value = arg.Substring(arg.IndexOf(':') + 1);

                    switch (key) {
                        case "/scripts":
                            settings.ScriptsFolderPath = AbsolutePath(value);
                            break;

                        case "/config":
                            settings.WebConfigPath = AbsolutePath(value);
                            break;

                        case "/conn":
                            settings.ConnectionStringName = value;
                            break;

                        case "/table":
                            settings.TableName = value;
                            break;

                        case "/user":
                            settings.DatabaseUserID = value;
                            break;

                        case "/password":
                            settings.DatabasePassword = value;
                            break;

                        case "/server":
                            settings.DatabaseDataSource = value;
                            break;

                        case "/catalog":
                            settings.DatabaseInitialCatalog = value;
                            break;

                        case "/timeout":
                            settings.DatabaseTimeout = Convert.ToInt32(value);
                            break;
                    }
                }
            }

            // TODO: let Program.cs do this checking..
            if (string.IsNullOrEmpty(settings.ScriptsFolderPath)) { throw new ArgumentNullException("scripts"); }
            if (string.IsNullOrEmpty(settings.TableName)) { throw new ArgumentNullException("table"); }

            if (string.IsNullOrEmpty(settings.WebConfigPath)) {

                var sb = new System.Data.SqlClient.SqlConnectionStringBuilder
                {
                    UserID = settings.DatabaseUserID,
                    Password = settings.DatabasePassword,
                    InitialCatalog = settings.DatabaseInitialCatalog,
                    DataSource = settings.DatabaseDataSource,
                    ConnectTimeout = settings.DatabaseTimeout
                };

                settings.ConnectionString = sb.ConnectionString;

            } else {

                if (!File.Exists(settings.WebConfigPath)) {
                    throw new FileNotFoundException(settings.WebConfigPath);
                }
                if (string.IsNullOrEmpty(settings.ConnectionStringName)) { throw new ArgumentNullException("/conn is required when specifying /config"); }

                ConfigurationParser cp = new ConfigurationParser(settings.WebConfigPath);
                settings.ConnectionString = cp.ConnectionString(settings.ConnectionStringName);
            }

            if (string.IsNullOrEmpty(settings.ConnectionString)) {
                throw new ApplicationException("Cannot determine database connection. Please specify either config/conn or user/password/server/catalog.");
            }

            return settings;
        }

        static string AbsolutePath(string path) {
            if (path.StartsWith("\\") || path.StartsWith(".")) {
                path = Path.Combine(Directory.GetCurrentDirectory(), path);
            }
            return path;
        }
    }
}
