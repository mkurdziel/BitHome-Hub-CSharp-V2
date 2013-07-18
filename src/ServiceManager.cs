using System;
using NLog;
using BitHome.Messaging;

namespace BitHome
{
	public static class ServiceManager
	{
		private static Logger log = LogManager.GetCurrentClassLogger();

		private static NodeService m_nodeService;
		private static MessageDispatcherService m_messageDispatcherService;
		private static StorageService m_storageService;

		private static Object m_lock = new object();
		private static Boolean m_started = false;
		private static Boolean m_isTesting = false;

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

			m_storageService = new StorageService (m_isTesting);
			m_messageDispatcherService = new MessageDispatcherService ();
			m_nodeService = new NodeService ();

			m_storageService.Start ();
			m_nodeService.Start ();
			m_messageDispatcherService.Start ();

			return true;
		}

		private static bool StopServiceManager() {
			log.Info ("Stopping ServiceManager");

			m_nodeService.Stop ();
			m_messageDispatcherService.Stop ();
			m_storageService.Stop ();

			return true;
		}

		public static NodeService NodeService {
			get { return m_nodeService; }
		}

		public static MessageDispatcherService MessageDispatcherService {
			get { return m_messageDispatcherService; }
		}

		public static StorageService StorageService {
			get { return m_storageService; }
		}
	}
}

