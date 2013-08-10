using System;

namespace BitHome
{
	[Serializable]
	public class Version : IComparable
	{
		public int MajorVersion {get; set;}
		public int MinorVersion { get; set; }
		public String VersionString { 
			get {
				return this.ToString ();
			}
		}
		public override string ToString ()
		{
			return string.Format ("{0}.{1}", MajorVersion, MinorVersion);
		}

		public Version() {
		}

		public Version( int majorVersion, int minorVersion) {
			MinorVersion = minorVersion;
			MajorVersion = majorVersion;
		}

		#region IComparable implementation
		public int CompareTo (object obj)
		{
			if (obj == null)
				return 1;

			Version otherVersion = obj as Version;
			if (otherVersion != null) {
				if (MajorVersion > otherVersion.MajorVersion) {
					return 1;
				}
				if (MajorVersion < otherVersion.MajorVersion) {
					return -1;
				}
				if (MinorVersion > otherVersion.MinorVersion) {
					return 1;
				}
				if (MinorVersion < otherVersion.MinorVersion) {
					return -1;
				}
				return 0;
			} else {
				throw new ArgumentException ("Object is not a Version");
			}
		}
		#endregion
	}
}

