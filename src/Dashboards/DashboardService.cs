using System;
using System.Collections.Generic;
using BitHome.Actions;
using ServiceStack.Text;
using NLog;

namespace BitHome.Dashboards 
{
	public class DashboardService : IBitHomeService
	{
		private const string KEY_DASHBOARDS = "dashboards";

		private static Logger log = LogManager.GetCurrentClassLogger();

		#region Member Variables
		Dictionary<string, Dashboard> m_dashboards = null;
		Dictionary<string, DashboardItem> m_dashboardItems;
		#endregion

		#region Properties
		public String[] DashboardIds {
			get {
				return m_dashboards.Keys.ToArray ();
			}
		}

		public Dashboard[] Dashboards { 
			get {
				return m_dashboards.Values.ToArray ();
			} 
		}
		#endregion

		#region Constructors 

		public DashboardService ()
		{
			m_dashboards = new Dictionary<string, Dashboard> ();
			m_dashboardItems = new Dictionary<string, DashboardItem> ();

			LoadData ();
		}

		private void LoadData() 
		{
			// Load data from the storage service
			if (StorageService.Store<String[]>.Exists(KEY_DASHBOARDS)) {
				String[] dashboardIds = StorageService.Store<String[]>.Get (KEY_DASHBOARDS);

				foreach (String key in dashboardIds) 
				{
					if (StorageService.Store<Dashboard>.Exists (key)) {
						m_dashboards.Add (key, StorageService.Store<Dashboard>.Get (key));
					} else {
						log.Warn ("Dashboard not found for key {0}", key);
					}
				}
			}
		}

		#endregion Constructors

		#region IBitHomeService implementation

		public bool Start ()
		{
			log.Info ("Starting DashboardService");

			return true;
		}

		public void Stop ()
		{
			log.Info ("Stopping DashboardService");
		}

		#endregion

		public void SetDashboardName (Dashboard dashboard, string name)
		{
			dashboard.Name = name;

			SaveDashboard (dashboard);
		}

		public Dashboard GetDashboard(String dashboardId) {
			if (m_dashboards.ContainsKey (dashboardId)) {
				return m_dashboards [dashboardId];
			}
			return null;
		}

		public Dashboard CreateDashboardFromNode (string nodeId, string name)
		{
			Dashboard dashboard = CreateDashboard (name);

			Node node = ServiceManager.NodeService.GetNode (nodeId);

			foreach (String actionId in node.Actions.Values) {
				IAction action = ServiceManager.ActionService.GetAction (actionId);
				DashboardItem item = new DashboardItem(StorageService.GenerateKey(), action);
				dashboard.AddItem (item);
			}

			return dashboard;
		}

		public Dashboard CreateDashboard(String name) {
			Dashboard dashboard = CreateDashboard ();

			if ( String.IsNullOrEmpty(name) == false) {
				SetDashboardName (dashboard, name);
			}

			return dashboard;
		}

		public Dashboard CreateDashboard() {
			Dashboard dashboard = new Dashboard (StorageService.GenerateKey());

			dashboard.Name = "Dashboard " + (m_dashboards.Count + 1);

			m_dashboards.Add (dashboard.Id, dashboard);

			SaveDashboard (dashboard);
			SaveDashboardList ();

			return dashboard;
		}

		public void RemoveDashboard(Dashboard dashboard) {
			if (dashboard != null) {
				foreach (DashboardItem item in dashboard.GetItems ()) {
					RemoveDashboardItem (item.Id);
				}

				m_dashboards.Remove (dashboard.Id);

				UnSaveDashboard (dashboard.Id);

				SaveDashboardList ();
			}
		}

		public void RemoveDashboardItem (string dashboardItemId)
		{
			UnSaveDashboardItem (dashboardItemId);
		}

		public DashboardItem GetDashboardItem (String id)
		{
			if (m_dashboardItems.ContainsKey (id)) {
				return m_dashboardItems[id];
			}
			return null;
		}

		public DashboardItem SetItemPosition (
			string dashboardItemId, 
			string positionX, 
			string positionY)
		{
			DashboardItem item = GetDashboardItem (dashboardItemId);
			if (item != null) {
				item.PositionX = positionX;
				item.PositionY = positionY;

				SaveDashboardItem (item);
			}

			return item;
		}

		#region Persistence Methods

		private void SaveDashboardList() {
			StorageService.Store<String[]>.Insert (KEY_DASHBOARDS, this.DashboardIds);
		}

		private void SaveDashboard(Dashboard dashboard) {
			StorageService.Store<Dashboard>.Insert (dashboard.Id, dashboard);
		}

		private void UnSaveDashboard(String dashboardId) {
			StorageService.Store<Dashboard>.Remove (dashboardId);
		}

		private void SaveDashboardItem(DashboardItem dashboardItem) {
			StorageService.Store<DashboardItem>.Insert (dashboardItem.Id, dashboardItem);
		}

		private void UnSaveDashboardItem(String dashboardItemId) {
			StorageService.Store<DashboardItem>.Remove (dashboardItemId);
		}

		public void WaitFinishSaving ()
		{
			StorageService.Store<String[]>.WaitForCompletion ();
			StorageService.Store<Dashboard>.WaitForCompletion ();
		}
		#endregion
	}
}

