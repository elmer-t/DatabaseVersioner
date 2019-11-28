using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseVersioner;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DatabaseVersioner.Tests {

    [TestClass()]
    public class SchemaVersionTests {

        [TestMethod()]
        public void CompareToTest() {

            SchemaVersion v1 = new SchemaVersion("00.00.00.0000");
            SchemaVersion v2 = new SchemaVersion("00.00.00.0000");
            int expected = 0;
            int actual = v1.CompareTo(v2);
            Assert.AreEqual(expected, actual);

            v1 = new SchemaVersion("00.00.00.0000");
            v2 = new SchemaVersion("00.00.00.0001");
            expected = -1;
            actual = v1.CompareTo(v2);
            Assert.AreEqual(expected, actual);

            v1 = new SchemaVersion("00.00.00.0001");
            v2 = new SchemaVersion("00.00.00.0000");
            expected = 1;
            actual = v1.CompareTo(v2);
            Assert.AreEqual(expected, actual);

            v1 = new SchemaVersion("00.00.01.0000");
            v2 = new SchemaVersion("00.00.00.0001");
            expected = 1;
            actual = v1.CompareTo(v2);
            Assert.AreEqual(expected, actual);

            v1 = new SchemaVersion("00.01.00.0000");
            v2 = new SchemaVersion("00.00.01.0001");
            expected = 1;
            actual = v1.CompareTo(v2);
            Assert.AreEqual(expected, actual);

            v1 = new SchemaVersion("01.00.01.0000");
            v2 = new SchemaVersion("00.00.01.0001");
            expected = 1;
            actual = v1.CompareTo(v2);
            Assert.AreEqual(expected, actual);

            v1 = new SchemaVersion("01.01.00.0000");
            v2 = null;
            expected = 1;
            actual = v1.CompareTo(v2);
            Assert.AreEqual(expected, actual);
            
        }
        
        [TestMethod()]
        public void SchemaVersionGreaterThan() {
            SchemaVersion v1 = new SchemaVersion("00.00.00.0000");
            SchemaVersion v2 = new SchemaVersion("00.00.00.0001");
            bool expected = false;
            bool actual = (v1 > v2);
            Assert.AreEqual(expected, actual);

            v1 = new SchemaVersion("00.00.00.0000");
            v2 = new SchemaVersion("00.00.00.0001");
            expected = false;
            actual = (v1 >= v2);
            Assert.AreEqual(expected, actual);

            v1 = new SchemaVersion("00.00.00.0001");
            v2 = new SchemaVersion("00.00.00.0001");
            expected = true;
            actual = (v1 >= v2);
            Assert.AreEqual(expected, actual);

            v1 = new SchemaVersion("02.00.00.0000");
            v2 = new SchemaVersion("01.99.99.9999");
            expected = true;
            actual = (v1 > v2);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void SchemaVersionSmallerThan() {
            SchemaVersion v1 = new SchemaVersion("00.00.00.0000");
            SchemaVersion v2 = new SchemaVersion("00.00.00.0001");
            bool expected = true;
            bool actual = (v1 < v2);
            Assert.AreEqual(expected, actual);

            v1 = new SchemaVersion("00.00.00.0001");
            v2 = new SchemaVersion("00.00.00.0001");
            expected = true;
            actual = (v1 <= v2);
            Assert.AreEqual(expected, actual);

            v1 = new SchemaVersion("00.00.00.0000");
            v2 = new SchemaVersion("00.00.00.0001");
            expected = true;
            actual = (v1 <= v2);
            Assert.AreEqual(expected, actual);

            v1 = new SchemaVersion("02.00.00.0000");
            v2 = new SchemaVersion("01.99.99.9999");
            expected = false;
            actual = (v1 < v2);
            Assert.AreEqual(expected, actual);
        }
    }
}
