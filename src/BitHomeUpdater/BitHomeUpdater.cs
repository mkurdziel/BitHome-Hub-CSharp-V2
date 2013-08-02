using System;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace BitHomeUpdater
{
	class BitHomeUpdater
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("BitHome Updater");
			if (args.Length > 0) {
				String cmd = args [0];

				if (cmd.Equals ("reboot")) {
					Reboot();
				}

				if (cmd.Equals ("update")) {
					if (args.Length != 3) {
						Console.Error.WriteLine ("Usage: BitHomeUpdater update currentDir updateDir");
						return;
					}

					String currentDir = args [1];
					String updateDir = args [2];

//					PerformUpdate (currentDir, updateDir);

				}
			}
		}

		static void PerformUpdate (string currentDir, string updateDir)
		{
			// First check if there is a previous install in the current dir
			String previousDir = Path.Combine(currentDir, "previousVersion");

			if ( File.Exists(previousDir)) {
				Console.WriteLine ("Deleting previous dir");
				File.Delete (previousDir);
			}

			// Create a directory to hold the previous install
			Directory.CreateDirectory (previousDir);

			// Perform the shutdown
			Shutdown ();

			CopyContents (currentDir, previousDir);
		}

		static void CopyContents(String from, String to) {
			foreach(var file in Directory.GetFiles(from))
				File.Copy(file, Path.Combine(to, Path.GetFileName(file)));

			foreach(var directory in Directory.GetDirectories(to)) {
				// Omit itself if necessary
				if (directory != to) {
					CopyContents(directory, Path.Combine(from, Path.GetFileName(directory)));
				}
			}
		}

		static void Reboot ()
		{
			Console.WriteLine ("Rebooting BitHome...");

			// Shut down the app via the shutdown file
			if (Shutdown ()) {
				StartBitHome ();
			}
		}

		static void StartBitHome() {
			Console.WriteLine ("Starting BitHome");

			ProcessStartInfo startInfo = new ProcessStartInfo ();

			switch (Environment.OSVersion.Platform) {
			case PlatformID.MacOSX:
				startInfo.FileName = "/bin/sh";
				startInfo.Arguments = "StartBitHome_Linux.sh";
				break;
			case PlatformID.Unix:
				startInfo.FileName = "/bin/sh";
				startInfo.Arguments = "StartBitHome_Linux.sh";
				break;
			case PlatformID.Win32S:
			case PlatformID.Win32NT:
			case PlatformID.Win32Windows:
				break;
			}
			System.Diagnostics.Process proc = new System.Diagnostics.Process();
			proc.EnableRaisingEvents = false;
			proc.StartInfo = startInfo;
			proc.Start();



			Console.WriteLine ("BitHome started pid:{0}", proc.Id);
		}

		static bool Shutdown() {
			try {

				String pid = GetPid ();

				if (pid != null) {
					File.Delete(BitHome.ServiceManager.PID_FILE);

					// Wait for the pid to be quit
					bool exited = false;
					for (int i=0; i<5; ++i) {
						try {
							Process.GetProcessById(Int32.Parse(pid));

							// Sleep then try again
							Thread.Sleep(TimeSpan.FromSeconds(1));
						} catch (Exception ) { 
							Console.WriteLine("Bithome exited successfully");
							exited = true;
							break;
						}
					}
					if (exited == false) {
						Console.WriteLine ("Could not exit BitHome. Could not restart");
					} else {
						return true;
					}
				}
			} catch (Exception e) {
				Console.WriteLine ("Could not delete shutdown file");
			}
			return false;
		}

		static String GetPid() {
			try {
				string text = File.ReadAllText (BitHome.ServiceManager.PID_FILE);
				return text;
			} catch (Exception e) {
				Console.WriteLine ("Could not read PID from file {0}", BitHome.ServiceManager.PID_FILE);
			}
			return null;
		}
	}
}
