using System;
using BinaryRage;
using NLog;
using System.Threading;
using System.IO;
using System.Collections.Generic;
using ServiceStack.Text;

namespace BitHome
{
	public class StorageService
	{
		private const string KEY_KEYS = "keys";
	    private const string APPDATA_FOLDER = "BitHome";
	    private const string DB_FILE = "db";

		private static Logger log = LogManager.GetCurrentClassLogger();
	    private static String m_path = null;
		private static HashSet<String> m_keys = new HashSet<String> ();

		public String[] AllKeys {
			get {
				return m_keys.ToArray ();
			}
		}

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

			// Load up the set of all keys
			if (Store<HashSet<String>>.Exists (KEY_KEYS)) {
				m_keys = Store<HashSet<String>>.Get (KEY_KEYS);
			} else {
				Store<HashSet<String>>.Insert (KEY_KEYS, m_keys, false);
				Store<HashSet<String>>.WaitForCompletion ();
			}

			Store<String>.Insert("version", ServiceManager.SettingsService.Version.VersionString);

			Store<String>.WaitForCompletion();
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


			log.Info ("DB at version {0}", Store<String>.Get("version"));

			return true;
		}

		public void Stop() {
			log.Info ("Stopping StorageService");
		}

		public static String GenerateKey() {
			return BinaryRage.Key.GenerateUniqueKey ();
		}

		public static class Store<T> {

			static public void Insert(string key, T value)
			{
				Insert (key, value, true);
			}

			static public void Insert(string key, T value, bool storeKey)
			{
				if (BinaryRage.DB<T>.Exists (key, m_path)) {
					BinaryRage.DB<T>.Remove (key, m_path);
					BinaryRage.DB<T>.WaitForCompletion();
				} else if (storeKey) {
					AddKey (key);
				}

				BinaryRage.DB<T>.Insert (key, value, m_path);
				BinaryRage.DB<T>.WaitForCompletion();
			}

			static public void Remove(string key)
			{
				if (BinaryRage.DB<T>.Exists (key, m_path)) {
					BinaryRage.DB<T>.Remove (key, m_path);
					BinaryRage.DB<T>.WaitForCompletion();
					RemoveKey (key);
				}
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

			private static void AddKey(String key) {
				m_keys.Add (key);
				StoreKeys ();
			}

			private static void RemoveKey(String key) {
				m_keys.Remove (key);
				StoreKeys ();
			}

			public static void StoreKeys() {
//				BinaryRage.DB<HashSet<String>>.Remove(KEY_KEYS, m_path);
//				BinaryRage.DB<HashSet<String>>.WaitForCompletion();
//				BinaryRage.DB<HashSet<String>>.Insert(KEY_KEYS, m_keys, m_path);
//				BinaryRage.DB<HashSet<String>>.WaitForCompletion();
			}
		}
	}
}

