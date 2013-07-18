using System;
using BinaryRage;
using NLog;
using System.Threading;
using System.IO;

namespace BitHome
{
	public class StorageService
	{
		private static Logger log = LogManager.GetCurrentClassLogger();
		private static String m_path = @"./appdata";

		public StorageService (bool p_isTesting)
		{
			// Set up a different path for testing data
			if (p_isTesting) {
				m_path = @"./testdata";
				// Delete the existing data
				if (Directory.Exists(m_path))
				{
					Directory.Delete (m_path, true);
				}
			}

			BinaryRage.DB<Double>.Insert ("version", 0.0, m_path);

			BinaryRage.DB<Double>.WaitForCompletion();
		}

		public StorageService ()
		{
			BinaryRage.DB<Double>.Insert ("version", 0.1, m_path);

            BinaryRage.DB<Double>.WaitForCompletion();
		}

		public Boolean Start() {
			log.Info ("Starting StorageService");

			log.Info ("DB at version {0}", BinaryRage.DB<Double>.Get("version", m_path));

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
		}
	}
}

