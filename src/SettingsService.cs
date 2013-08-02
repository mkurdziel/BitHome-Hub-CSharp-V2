using System;
using NLog;
using ServiceStack.Text;

namespace BitHome
{
	public static class SettingsService
	{
		private static int VERSION_MAJOR = 0;
		private static int VERSION_MINOR = 2;
		private static String VERSION_URL = "http://bithome.org/version.html";
		private static String UPDATE_EXE = "BitHomeUpdater.exe";

		private static Logger log = LogManager.GetCurrentClassLogger();

		public static Version Version {
			get {
				return new Version {
					MajorVersion = VERSION_MAJOR,
					MinorVersion = VERSION_MINOR
				};
			}
		}

		public static Version NewestVersion {
			get {
				Version newVersion = Version;
				using (var webClient = new System.Net.WebClient()) {
					String json = webClient.DownloadString(VERSION_URL);
					newVersion = json.FromJson<Version>();
				}
				return newVersion;
			}
		}

		public static void PerformUpdate() {
			System.Diagnostics.Process proc = new System.Diagnostics.Process();
			proc.EnableRaisingEvents = false;
			proc.StartInfo.FileName = UPDATE_EXE;
			proc.Start();
		}
	}
}

