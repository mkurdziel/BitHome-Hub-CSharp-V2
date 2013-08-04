using System;
using BinaryRage;
using NLog;
using System.Threading;
using System.IO;

namespace BitHome
{
	public class StorageService
	{
	    private const string APPDATA_FOLDER = "BitHome";
	    private const string DB_FILE = "db";

		private static Logger log = LogManager.GetCurrentClassLogger();
	    private static String m_path = null;

		public StorageService (bool p_isTesting)
		{
			// Set up a different path for testing data
			if (p_isTesting) {
				m_path = @"./testdata";

				DeleteDb ();
			} else {
                // Get the users application data path
                String appdatafolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			    String dbLocation = Path.Combine(Path.Combine(appdatafolder, APPDATA_FOLDER), DB_FILE);

			    log.Debug("Application data path: {0}", dbLocation);

			    m_path = dbLocation;
			}

			BinaryRage.DB<String>.Insert("version", ServiceManager.SettingsService.Version.VersionString, m_path);

			BinaryRage.DB<String>.WaitForCompletion();
		}

		public void DeleteDb() {
			log.Warn("Deleting db: {0}", m_path);

			// Delete the existing data
			if (Directory.Exists(m_path))
			{
				Directory.Delete (m_path, true);
			}
		}

		public Boolean Start() {
			log.Info ("Starting StorageService");


			log.Info ("DB at version {0}", BinaryRage.DB<String>.Get("version", m_path));

			return true;
		}

		public void Stop() {
			log.Info ("Stopping StorageService");
		}

		public static String GenerateKey() {
			return BinaryRage.Key.GenerateUniqueKey ();
		}

		public static void WaitForCompletion() {
			BinaryRage.DB<bool>.WaitForCompletion();
		}

		public static class Store<T> {

			static public void Insert(string key, T value)
			{
				if (BinaryRage.DB<T>.Exists (key, m_path))
				{
					BinaryRage.DB<T>.Remove (key, m_path);
				}

				BinaryRage.DB<T>.Insert (key, value, m_path);
			}

			static public void Remove(string key)
			{
				BinaryRage.DB<T>.Remove (key, m_path);
			}

			static public T Get(string key )
			{
				return BinaryRage.DB<T>.Get (key, m_path);
			}

			static public bool Exists(string key)
			{
				return BinaryRage.DB<T>.Exists (key, m_path);
			}

			static public void WaitForCompletion()
			{
				BinaryRage.DB<T>.WaitForCompletion ();
			}
		}
	}
}

