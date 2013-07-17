using System;
using BinaryRage;
using NLog;
using System.Threading;

namespace BitHome
{
	public class StorageService
	{
		private static Logger log = LogManager.GetCurrentClassLogger();
		private static String m_path = @"./appdata";


		public StorageService ()
		{
			BinaryRage.DB<Double>.Insert ("version", 0.1, m_path);

			Thread.Sleep (5000);

		}

		public Boolean Start() {
			log.Info ("Starting StorageService");

			log.Info ("DB at version {0}", BinaryRage.DB<Double>.Get("version", m_path));

			return true;
		}

		public void Stop() {
			log.Info ("Stopping StorageService");
		}
	}
}

