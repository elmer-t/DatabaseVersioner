using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseVersioner {

    public class SchemaVersion : IComparable<SchemaVersion> {

        public int Major { get; set; }
        public int Minor { get; set; }
        public int Revision { get; set; }
        public int Build { get; set; }
        public bool IsInitialized { get; set; }

        public SchemaVersion() {
            Initialize(0, 0, 0, 0, false);
        }

        public SchemaVersion(string version) {

            if (string.IsNullOrEmpty(version) || version.IndexOf(".") == -1) {
                Initialize(0, 0, 0, 0);
            }

            string[] parts = version.Split('.');
            Initialize(Convert.ToInt32(parts[0]), Convert.ToInt32(parts[1]), Convert.ToInt32(parts[2]), Convert.ToInt32(parts[3]));
        }

        public SchemaVersion(int major, int minor, int revision, int build) {
            Initialize(major, minor, revision, build);
        }

        private void Initialize(int major, int minor, int revision, int build, bool isInitialized = true) {
            this.Major = major;
            this.Minor = minor;
            this.Revision = revision;
            this.Build = build;
            this.IsInitialized = isInitialized;
        }

        public override string ToString() {
            return string.Format(
                "{0}.{1}.{2}.{3}",
                Major.ToString().PadLeft(2, '0'),
                Minor.ToString().PadLeft(2, '0'),
                Revision.ToString().PadLeft(2, '0'),
                Build.ToString().PadLeft(4, '0')
            );
        }

        public static bool operator ==(SchemaVersion v1, SchemaVersion v2) {
            return v1.CompareTo(v2) == 0;
        }

        public static bool operator !=(SchemaVersion v1, SchemaVersion v2) {
            return v1.CompareTo(v2) != 0;
        }

        public static bool operator <(SchemaVersion v1, SchemaVersion v2) {
            return v1.CompareTo(v2) < 0; ;
        }

        public static bool operator <=(SchemaVersion v1, SchemaVersion v2) {
            return v1.CompareTo(v2) <= 0;
        }

        public static bool operator >(SchemaVersion v1, SchemaVersion v2) {
            return v1.CompareTo(v2) > 0;
        }
        public static bool operator >=(SchemaVersion v1, SchemaVersion v2) {
            return v1.CompareTo(v2) >= 0;
        }

        public override bool Equals(object obj) {
            return (obj is SchemaVersion) && (this.CompareTo((SchemaVersion)obj) == 0);
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }

        public int CompareTo(SchemaVersion other) {

            if ((object)other == null) { return 1; }

            var value = Build + (Revision * 10000) + (Minor * 1000000) + (Major * 100000000);
            var otherValue = other.Build + (other.Revision * 10000) + (other.Minor * 1000000) + (other.Major * 100000000);

            if (value == otherValue) {
                return 0;
            } else if (value < otherValue) {
                return -1;
            } else {
                return 1;
            }
        }

        public static bool TryParse(string p, out SchemaVersion v) {
            try {
                v = new SchemaVersion(p);
                return true;
            } catch (Exception) {
                v = null;
                return false;
            }
        }
    }
}
