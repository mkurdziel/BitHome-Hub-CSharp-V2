using System;
using System.IO;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Tar;

namespace BitHome
{
	public static class FileHelpers
	{
		public static string GetTemporaryDirectory()
		{
			string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
			Directory.CreateDirectory(tempDirectory);
			return tempDirectory;
		}

		public static void ExtractGZip(string gzipFileName, string targetDir) {

			// Use a 4K buffer. Any larger is a waste.    
			byte[ ] dataBuffer = new byte[4096];

			using (System.IO.Stream fs = new FileStream(gzipFileName, FileMode.Open, FileAccess.Read)) {
				using (GZipInputStream gzipStream = new GZipInputStream(fs)) {

					// Change this to your needs
					string fnOut = Path.Combine(targetDir, Path.GetFileNameWithoutExtension(gzipFileName));

					using (FileStream fsOut = File.Create(fnOut)) {
						StreamUtils.Copy(gzipStream, fsOut, dataBuffer);
					}
				}
			}
		}

		public static void ExtractTar(String tarFileName, String destFolder) {

			Stream inStream = File.OpenRead(tarFileName);

			TarArchive tarArchive = TarArchive.CreateInputTarArchive(inStream);
			tarArchive.ExtractContents(destFolder);
			tarArchive.Close();

			inStream.Close();
		}
	}
}

