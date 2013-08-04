using System;
using NLog;
using BitHome.Messaging;
using BitHome.Actions;
using BitHome.Dashboards;
using System.Diagnostics;
using System.Timers;
using System.IO;

namespace BitHome
{
	public static class ServiceManager
	{
		public const String PID_FILE = "BitHome.pid";

		private static Logger log = LogManager.GetCurrentClassLogger();

		private static Object m_lock = new object();
		private static Boolean m_started = false;
		private static Boolean m_isTesting = false;
		private static Timer m_pidTimer;

		public static bool IsStarted { get { return m_started; } }

		public static bool Start(bool p_isTesting) {
			m_isTesting = p_isTesting;

			return Start ();
		}

		public static bool Start() {
			log.Trace ("Start requested at PID: {0}", Process.GetCurrentProcess().Id);

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

		// Restart the services without the storage
		public static bool RestartServices() {
			NodeService.Stop ();
			ActionService.Stop ();
			MessageDispatcherService.Stop ();

			SettingsService = new SettingsService ();
			MessageDispatcherService = new MessageDispatcherService (m_isTesting);
			NodeService = new NodeService ();
			ActionService = new ActionService();
			DashboardService = new DashboardService ();

			MessageDispatcherService.Start ();
			ActionService.Start ();
			NodeService.Start ();

			return true;
		}


		private static bool StartServiceManager() {
			log.Info ("Starting ServiceManager");

			// Create a shutdown file containing the pid
			CreatePidFile ();
			// Start the file shutdown watcher
			m_pidTimer = new Timer ();
			m_pidTimer.AutoReset = true;
			m_pidTimer.Elapsed += PidWatcherTimerTick;
			m_pidTimer.Interval = TimeSpan.FromSeconds (1).TotalMilliseconds;
			m_pidTimer.Start ();


			SettingsService = new SettingsService ();
			StorageService = new StorageService (m_isTesting);
			MessageDispatcherService = new MessageDispatcherService (m_isTesting);
			NodeService = new NodeService ();
			ActionService = new ActionService();
			DashboardService = new DashboardService ();

			StorageService.Start ();
			MessageDispatcherService.Start ();
			ActionService.Start ();
			NodeService.Start ();
			DashboardService.Start ();

			return true;
		}

		private static bool StopServiceManager() {
			log.Info ("Stopping ServiceManager");

			// Stop the pid watcher timer
			m_pidTimer.Stop ();

			NodeService.Stop ();
			ActionService.Stop ();
			MessageDispatcherService.Stop ();
			StorageService.Stop ();
			DashboardService.Stop ();

			return true;
		}

		static void CreatePidFile ()
		{
			// Delete the file if it exists
			if ( File.Exists(PID_FILE) ) {
				File.Delete (PID_FILE);
			}

			using (StreamWriter outfile = new StreamWriter(PID_FILE))
			{
				outfile.Write (Process.GetCurrentProcess ().Id);
			}
		}

		static void PidWatcherTimerTick (object sender, System.Timers.ElapsedEventArgs e)
		{
			if (File.Exists (PID_FILE) == false) {
				log.Info ("Shutdown file is missing. Shutting down BitHome");
				ServiceManager.Stop ();
			}
		}

		public static NodeService NodeService { get; private set; }

		public static MessageDispatcherService MessageDispatcherService { get; private set; }

		public static StorageService StorageService { get; private set; }

		public static ActionService ActionService { get; private set; }

		public static DashboardService DashboardService { get; private set; }

		public static SettingsService SettingsService { get; private set; }
	}
}

