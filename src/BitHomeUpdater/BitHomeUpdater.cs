using System;
using System.Collections.Generic;
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
					Reboot("");
				}

				if (cmd.Equals ("update")) {
					if (args.Length != 3) {
						Console.Error.WriteLine ("Usage: BitHomeUpdater update currentDir updateDir");
						return;
					}

					String currentDir = args [1];
					String updateDir = args [2];

					PerformUpdate (currentDir, updateDir);
				}
			}
		}

		static void PerformUpdate (string currentDir, string updateDir)
		{
			// First check if there is a previous install in the current dir
			String previousDir = Path.Combine(currentDir, "previousVersion");

			if ( Directory.Exists(previousDir)) {
				Console.WriteLine ("Deleting previous dir");
                Directory.Delete(previousDir, true);
			}

			// Create a directory to hold the previous install
			Directory.CreateDirectory (previousDir);

			// Perform the shutdown
			Shutdown (currentDir);

			MoveContents (currentDir, previousDir);

            CopyContents(Path.Combine(updateDir, "BitHome"), currentDir);

            StartBitHome(currentDir);
		}

		static void MoveContents(String from, String to) {
            //Now Create all of the directories
		    String[] directories = Directory.GetDirectories(from, "*", SearchOption.AllDirectories);

            List<String> dirList = new List<string>(directories);

            // Remove the destination directory
		    dirList.Remove(to);

            foreach (string dirPath in dirList)
            {
                 Console.WriteLine("Creating directory {0}", dirPath);
                 Directory.CreateDirectory(dirPath.Replace(from, to));
            }

		    //Copy all the files
            foreach (string newPath in Directory.GetFiles(from, "*.*",
                SearchOption.AllDirectories))
            {
                Console.WriteLine("Moving file {0}", newPath);
                File.Move(newPath, newPath.Replace(from, to));
            }

		    // Remove all the previous directories
            foreach (string dirPath in dirList)
            {
                Console.WriteLine("Removing old directory {0}", dirPath);
                try
                {
                    Directory.Delete(dirPath, true);
                } catch (DirectoryNotFoundException) {
                    Console.Error.WriteLine("Dir not found {0}", dirPath);
                }
            }
		}

        static void CopyContents(String from, String to)
        {
            //Now Create all of the directories
            String[] directories = Directory.GetDirectories(from, "*", SearchOption.AllDirectories);

            List<String> dirList = new List<string>(directories);

            // Remove the destination directory
            dirList.Remove(to);

            foreach (string dirPath in dirList)
            {
                Console.WriteLine("Creating directory {0}", dirPath);
                Directory.CreateDirectory(dirPath.Replace(from, to));
            }

            //Copy all the files
            foreach (string newPath in Directory.GetFiles(from, "*.*",
                SearchOption.AllDirectories))
            {
                Console.WriteLine("Copying file {0}", newPath);
                File.Copy(newPath, newPath.Replace(from, to));
            }
        }

		static void Reboot (string currentDir)
		{
			Console.WriteLine ("Rebooting BitHome...");

			// Shut down the app via the shutdown file
			if (Shutdown (currentDir)) {
				StartBitHome (currentDir);
			}
		}

		static void StartBitHome(string currentDir) {
			Console.WriteLine ("Starting BitHome");

			ProcessStartInfo startInfo = new ProcessStartInfo ();

			switch (Environment.OSVersion.Platform) {
			case PlatformID.MacOSX:
				startInfo.FileName = "/bin/sh";
				startInfo.Arguments = Path.Combine(currentDir, "StartBitHome_Linux.sh");
				break;
			case PlatformID.Unix:
				startInfo.FileName = "/bin/sh";
				startInfo.Arguments =  Path.Combine(currentDir, "StartBitHome_Linux.sh");
				break;
			case PlatformID.Win32S:
			case PlatformID.Win32NT:
			case PlatformID.Win32Windows:
				startInfo.FileName =  Path.Combine(currentDir, "StartBitHome_Windows.bat");
				break;
			}
			System.Diagnostics.Process proc = new System.Diagnostics.Process();
			proc.EnableRaisingEvents = false;
			proc.StartInfo = startInfo;
			proc.Start();


			Console.WriteLine ("BitHome started pid:{0}", proc.Id);
		}

		static bool Shutdown(string currentDir) {
			try {

				String pid = GetPid (currentDir);

				if (pid != null) {
					File.Delete(Path.Combine(currentDir, BitHome.ServiceManager.PID_FILE));

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
			} catch (Exception) {
				Console.WriteLine ("Could not delete shutdown file");
			}
			return false;
		}

		static string GetPid(string currentDir) {
			try {
				string text = File.ReadAllText (Path.Combine(currentDir, BitHome.ServiceManager.PID_FILE));
				return text;
			} catch (Exception) {
				Console.WriteLine ("Could not read PID from file {0}", BitHome.ServiceManager.PID_FILE);
			}
			return null;
		}
	}
}
