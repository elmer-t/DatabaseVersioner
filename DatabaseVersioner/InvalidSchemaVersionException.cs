using System;

namespace DatabaseVersioner {

    public class InvalidSchemaVersionException : Exception {
        private readonly SchemaVersion _expectedVersion;
        private readonly SchemaVersion _actualVersion;

        public InvalidSchemaVersionException(SchemaVersion expected, SchemaVersion actual) {
            _expectedVersion = expected;
            _actualVersion = actual;
        }

        public override string Message {
            get {
                return string.Format("Expecting schema version {0} but got {1}", _expectedVersion, _actualVersion);
            }
        }
    }
}
