using System;
using NLog;
using ServiceStack.Text;
using System.ComponentModel;
using System.Net;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Core;

namespace BitHome
{
	public class SettingsService
	{
		private static int VERSION_MAJOR = 0;
		private static int VERSION_MINOR = 2;
		public static String BITHOME_URL = "http://bithome.org";
		public static String VERSION_URL = BITHOME_URL + "/version.html";
		public static String BITHOME_FILE = "BitHome.tar.gz";
		public static String UPDATE_URL = BITHOME_URL + "/" + BITHOME_FILE;
		public static String UPDATE_EXE = "BitHomeUpdater.exe";

		private Logger log = LogManager.GetCurrentClassLogger();
		private object m_downloadLock = new object();
		private String m_tempDirectory;

		public Version Version {
			get {
				return new Version {
					MajorVersion = VERSION_MAJOR,
					MinorVersion = VERSION_MINOR
				};
			}
		}

		public Version NewestVersion {
			get {
				Version newVersion = Version;
				using (var webClient = new System.Net.WebClient()) {
					String json = webClient.DownloadString(VERSION_URL);
					newVersion = json.FromJson<Version>();
				}
				return newVersion;
			}
		}

		public Boolean IsDownloading { get; private set; }

		public SettingsService() {
		}

		public void PerformReboot ()
		{
			System.Diagnostics.Process proc = new System.Diagnostics.Process();
			proc.EnableRaisingEvents = false;
			proc.StartInfo.FileName = UPDATE_EXE;
			proc.StartInfo.Arguments = "reboot";
			proc.Start();		
		}

		public void DownloadUpdateFile() {
			if (IsDownloading == false) {
				lock(m_downloadLock) {
					// Create a temp directory to download the file to
					m_tempDirectory = FileHelpers.GetTemporaryDirectory ();

					log.Info ("Downloading update file to {0}", m_tempDirectory);

					IsDownloading = true;

					using (var webClient = new System.Net.WebClient()) {
						webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler (DownloadProgressChanged);
						webClient.DownloadFileCompleted += new AsyncCompletedEventHandler (DownloadFileCompleted);
						webClient.DownloadFileAsync (
							new Uri (UPDATE_URL), 
							Path.Combine (m_tempDirectory, BITHOME_FILE)
						);
					}
				}
			} else {
				log.Warn ("Trying to download update when already downloading");
			}
		}


		private void DownloadFileCompleted (object sender, AsyncCompletedEventArgs e)
		{
			log.Info ("Completed update file download to {0}", m_tempDirectory);


			ExtractUpdateFile ();

			RunUpdate ();

			IsDownloading = false;
		}

		private void ExtractUpdateFile() {
			log.Info ("Update extracting gzip file {0}", BITHOME_FILE);

			FileHelpers.ExtractGZip(Path.Combine(m_tempDirectory, BITHOME_FILE), m_tempDirectory);

			String tarFile = BITHOME_FILE.Replace (".gz", "");

			log.Info ("Update extracting tar file {0}", tarFile);

			FileHelpers.ExtractTar (Path.Combine (m_tempDirectory, tarFile), m_tempDirectory);

			log.Info ("Update extraction complete");
		}

		private void DownloadProgressChanged (object sender, DownloadProgressChangedEventArgs e)
		{
			log.Trace ("Download update progress: {0}%", e.ProgressPercentage);
		}

		private void RunUpdate() {
			String updateDir = Path.Combine (m_tempDirectory, "BitHome");
			String updateFile = Path.Combine (updateDir, UPDATE_EXE);

			if (!File.Exists (updateFile)) {
				log.Warn ("Updater application does not exist");
				return;
			}

			String updateArgs = String.Format ("update B{0} {1}", Environment.CurrentDirectory, m_tempDirectory);
			log.Debug ("Executing update file: {0} {1}", updateFile, updateArgs);

			// Set to be executable
			File.SetAttributes (updateFile, (FileAttributes)((int) File.GetAttributes (updateFile) | 0x8000000));

			System.Diagnostics.Process proc = new System.Diagnostics.Process();
			proc.EnableRaisingEvents = false;
			proc.StartInfo.FileName = updateFile;
			proc.StartInfo.Arguments = updateArgs;
			proc.StartInfo.RedirectStandardOutput = true;
			proc.StartInfo.RedirectStandardError = true;
			proc.StartInfo.UseShellExecute = false;
			proc.Start();
		}

		public void PerformUpdate() {
			DownloadUpdateFile ();
		}
	}
}

