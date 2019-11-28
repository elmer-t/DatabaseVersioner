using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace DatabaseVersioner {

    /// <summary>
    /// Parses a web.config file and extracts various bit of information from it.
    /// </summary>
    public class ConfigurationParser {

        protected string Path { get; set; }

        public ConfigurationParser(string path) {
            this.Path = path;
        }

        /// <summary>
        /// Returns the specified connection string
        /// </summary>
        /// <returns></returns>
        public string ConnectionString(string connectionStringName) {

            XDocument doc = XDocument.Load(Path);

            var connections = doc.Descendants("connectionStrings").Descendants();
            foreach (XElement conn in connections) {
                if (conn.Attribute("name").Value == connectionStringName) {
                    return conn.Attribute("connectionString").Value;
                }
            }

            return null;

        }
    }
}
