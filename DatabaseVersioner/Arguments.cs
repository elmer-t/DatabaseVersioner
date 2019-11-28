using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseVersioner {

    public class Arguments {

        public string ScriptsFolderPath { get; set; }
        public string WebConfigPath { get; set; }
        public string ConnectionStringName { get; set; }
        public string TableName { get; set; }

        public string DatabaseUserID { get; set; }
        public string DatabasePassword { get; set; }
        public string DatabaseDataSource { get; set; }
        public string DatabaseInitialCatalog { get; set; }
        public int DatabaseTimeout { get; set; }

        public string ConnectionString { get; set; }

        public Arguments() {
            SetDefaults();
        }

        /// <summary>
        /// Set default values
        /// </summary>
        protected void SetDefaults() {
            TableName = "SchemaChanges";
            DatabaseTimeout = 15;
        }
    }
}
