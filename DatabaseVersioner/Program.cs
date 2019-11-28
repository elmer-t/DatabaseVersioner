using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseVersioner
{
    internal class Program
    {
        private enum ExitCode
        {
            Success = 0,
            Failed = 1
        }

        private static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            ResetConsoleColors();
            WriteCopyright();

            if (!args.Any())
            {
                Help();
            }
            else
            {
                Arguments settings = ArgumentParser.Parse(args);

                //Console.WriteLine("Connection string : " + settings.ConnectionString);

                using (DataSource ds = new DataSource(settings))
                {

                    if (ds.SchemaChangesTableExists() == false) {

                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"Initializing dbo.{ds.TableName}...");
                        Console.ForegroundColor = ConsoleColor.White;

                        ds.CreateSchemaChangesTable();
                    }

                    Console.WriteLine("Executed       : {0} from {1}", DateTime.Now, ds.WorkStationId);
                    Console.WriteLine("Database       : {0} on {1}", ds.DatabaseName, ds.DataSourceName);
                    Console.WriteLine("SQL Server     : {0}", ds.ServerVersion);
                    Console.WriteLine("Schema version : {0}", ds.GetSchemaVersion());
                    Console.WriteLine("Schema table   : [{0}]", ds.TableName);
                    Console.WriteLine();
                }

                var returnCode = ExecuteScripts(settings);

                using (DataSource ds = new DataSource(settings))
                {
                    Console.WriteLine();
                    Console.WriteLine("Schema version : {0}", ds.GetSchemaVersion());
                    Console.WriteLine();
                }

                Console.WriteLine("All done");
                
                Environment.Exit((int) returnCode);
            }
        }

        private static void WriteCopyright()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(asm.Location);

            Console.WriteLine("{0} v.{1} - Keep your database schemas in sync", fvi.ProductName,
                Assembly.GetExecutingAssembly().GetName().Version);

            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Red;
            Console.Write("REDHEAD");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.BackgroundColor = ConsoleColor.White;
            Console.Write("IT");

            ResetConsoleColors();

            Console.WriteLine(" (http://redheadit.nl)");
            Console.WriteLine();
        }

        private static void ResetConsoleColors()
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.BackgroundColor = ConsoleColor.Black;
        }

        private static ExitCode ExecuteScripts(Arguments settings)
        {
            if (!Directory.Exists(settings.ScriptsFolderPath))
            {
                throw new DirectoryNotFoundException(settings.ScriptsFolderPath);
            }

            var scriptFilePaths = Directory.GetFiles(settings.ScriptsFolderPath, "*.sql");
            Array.Sort(scriptFilePaths);

            using (var ds = new DataSource(settings))
            {
                if (!scriptFilePaths.Any())
                {
                    Console.WriteLine("No scripts found in {0}", settings.ScriptsFolderPath);
                }

                // Execute change scripts
                foreach (string scriptFilePath in scriptFilePaths)
                {
                    SchemaVersion version;
                    var scriptFileName = Path.GetFileNameWithoutExtension(scriptFilePath);

                    // Only process scripts where the name starts with a valid SchemaVersion
                    if (scriptFileName == null || !SchemaVersion.TryParse(scriptFileName.Substring(0, 13), out version))
                    {
                        continue;
                    }

                    Console.WriteLine("Processing {0}...", Path.GetFileName(scriptFilePath));

                    try
                    {
                        var script = File.ReadAllText(scriptFilePath);
                        ds.ExecuteScript(script, version, scriptFileName.Substring(13).Trim(" -_.".ToCharArray()));

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("  Done");
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                    catch (InvalidSchemaVersionException ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("  " + ex.Message);
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                    catch (DuplicateScriptException ex)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("  " + ex.Message);
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("  Error while processing {0}", Path.GetFileName(scriptFilePath));
                        Console.WriteLine("  " + ex.Message);
                        Console.WriteLine("  Not processing further scripts!");
                        Console.ForegroundColor = ConsoleColor.Gray;

                        // Don't process further scripts
                        return ExitCode.Failed;
                    }
                }
            }

            return ExitCode.Success;
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(e.ExceptionObject.ToString());
            Console.WriteLine();

            ResetConsoleColors();

            //Console.WriteLine("Press [enter] to continue");
            //Console.ReadLine();

            Environment.Exit((int) ExitCode.Failed);
        }

        private static void Help()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(asm.Location);

            Console.WriteLine("USAGE");
            Console.WriteLine(
                "  {0} /scripts:[path] /config:[path] [/conn:[name]] [/user:[name]] [/password:[password]] [/server:[name|ip]] [/catalog:[name]] [/table:name]",
                Path.GetFileName(fvi.FileName));
            Console.WriteLine();
            Console.WriteLine("  /scripts  Path to folder containing change scripts");
            Console.WriteLine("            Script file names _must_ start with a valid schema versions (00.00.0000)");
            Console.WriteLine("            and have a .sql file extension. Any text following the version nr is");
            Console.WriteLine("            inserted in the SchemaChanges table as Description");
            Console.WriteLine("            Relative paths must start with \\");
            Console.WriteLine();
            Console.WriteLine("  /config   Path to web.config containing connection string");
            Console.WriteLine("            Relative paths must start with \\");
            Console.WriteLine();
            Console.WriteLine("  /conn     Name of the connection string from web.config to use");
            Console.WriteLine();
            Console.WriteLine("  /user     Username for db connection");
            Console.WriteLine();
            Console.WriteLine("  /password Password for db connection");
            Console.WriteLine();
            Console.WriteLine("  /server   Name/IP of database server");
            Console.WriteLine();
            Console.WriteLine("  /catalog  Name of database catalog");
            Console.WriteLine();
            Console.WriteLine("  /table    Name of the schema version table, [SchemaChanges] by default");
            Console.WriteLine();
            Console.WriteLine(
                "Database connection is either specified by /conn or combination of /user, /password, /server and /catalog");

        }
    }
}