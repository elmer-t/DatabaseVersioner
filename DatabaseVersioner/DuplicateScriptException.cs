using System;

namespace DatabaseVersioner {

    public class DuplicateScriptException: Exception {
        private readonly SchemaVersion _version;

        public DuplicateScriptException(SchemaVersion version)
        {
            _version = version;
        }

        public override string Message {
            get {
                return string.Format("Script version {0} has already been processed", _version);
            }
        }
    }
}
