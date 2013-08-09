using System;
using ServiceStack.ServiceHost;

namespace BitHome.WebApi
{
	[Route("/api/settings/reboot", "GET")]
	public class WebApiRebootResponse : IReturn<bool> { }

	[Route("/api/settings/version", "GET")]
	public class WebApiVersionResponse : IReturn<Version> { }

	[Route("/api/settings/version/newest", "GET")]
	public class WebApiNewestVersionResponse : IReturn<Version> { }

	[Route("/api/settings/version/update", "GET")]
	public class WebApiUpdateVersionResponse : IReturn<Version> { }

	public class SettingsWebApiService : ServiceStack.ServiceInterface.Service
	{
		public SettingsService SettingsService {
			get {
				return ServiceManager.SettingsService;
			}
		}
		public Version Get(WebApiVersionResponse request) {
			return SettingsService.Version;
		}

		public Version Get(WebApiNewestVersionResponse request) {
			return SettingsService.NewestVersion;
		}
		
		public Version Get(WebApiUpdateVersionResponse request) {
			SettingsService.PerformUpdate();

			return SettingsService.NewestVersion;
		}

		public bool Get(WebApiRebootResponse request) {
			SettingsService.PerformReboot();

			return true;
		}
	}
}

