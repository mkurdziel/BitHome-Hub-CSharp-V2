using System;
using NLog;
using BitHome.Messaging;
using BitHome.Actions;

namespace BitHome
{
	public static class ServiceManager
	{
		private static Logger log = LogManager.GetCurrentClassLogger();

		private static Object m_lock = new object();
		private static Boolean m_started = false;
		private static Boolean m_isTesting = false;

		public static bool IsStarted { get { return m_started; } }

		public static bool Start(bool p_isTesting) {
			m_isTesting = p_isTesting;

			return Start ();
		}

		public static bool Start() {
			log.Trace ("Start requested");

			lock(m_lock) {
				// only allow the service manager to be started once
				if (m_started) {
					return false;
				} else {
					m_started = StartServiceManager ();
				}
			}

			return m_started;
		}

		public static bool Stop() {
			log.Trace ("Stop requested");

			lock (m_lock) {
				// Make sure it is running
				if (!m_started) {
					return false;
				} else {
					bool stopped = StopServiceManager ();

					m_started = !stopped;

					return stopped;
				}
			}
		}

		private static bool StartServiceManager() {
			log.Info ("Starting ServiceManager");

			StorageService = new StorageService (m_isTesting);
			MessageDispatcherService = new MessageDispatcherService (m_isTesting);
			NodeService = new NodeService ();
			ActionService = new ActionService();

			StorageService.Start ();
			MessageDispatcherService.Start ();
			ActionService.Start ();
			NodeService.Start ();

			return true;
		}

		private static bool StopServiceManager() {
			log.Info ("Stopping ServiceManager");

			NodeService.Stop ();
			ActionService.Stop ();
			MessageDispatcherService.Stop ();
			StorageService.Stop ();

			return true;
		}

		public static NodeService NodeService { get; private set; }

		public static MessageDispatcherService MessageDispatcherService { get; private set; }

		public static StorageService StorageService { get; private set; }

		public static ActionService ActionService { get; private set; }
	}
}

