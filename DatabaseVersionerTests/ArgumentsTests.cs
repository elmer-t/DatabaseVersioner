using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseVersioner;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DatabaseVersioner.Tests {

    [TestClass()]
    public class ArgumentsTests {
        
        [TestMethod()]
        public void ArgumentsTestNoParams() {

            Arguments settings = new Arguments();
                        
            Assert.IsNull(settings.ConnectionString);
            Assert.IsNull(settings.ConnectionStringName);
            Assert.IsNull(settings.DatabaseDataSource);
            Assert.IsNull(settings.DatabaseInitialCatalog);
            Assert.IsNull(settings.DatabasePassword);
            Assert.IsNull(settings.DatabaseUserID);
            Assert.IsNull(settings.ScriptsFolderPath);
            Assert.AreEqual(settings.TableName, "SchemaChanges");
            Assert.IsNull(settings.WebConfigPath);

        }

        [TestMethod()]
        public void ArgumentsTestWebConfig() {

            string[] args = { "test" };

            try {
                Arguments settings = ArgumentParser.Parse(args);
                Assert.Fail();
            } catch (ArgumentNullException) {
                
            }

            args = new string[] { "/config:...", "/scripts:..." };

        }
    }
}
