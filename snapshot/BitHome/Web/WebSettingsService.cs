using System;
using ServiceStack.ServiceHost;

namespace BitHome
{
	[Route("/api/settings/reboot", "GET")]
	public class WebRebootResponse : IReturn<bool> { }

	[Route("/api/settings/version", "GET")]
	public class WebVersionResponse : IReturn<Version> { }

	[Route("/api/settings/version/newest", "GET")]
	public class WebNewestVersionResponse : IReturn<Version> { }

	[Route("/api/settings/version/update", "GET")]
	public class WebUpdateVersionResponse : IReturn<Version> { }

	public class WebSettingsService : ServiceStack.ServiceInterface.Service
	{
		public SettingsService SettingsService {
			get {
				return ServiceManager.SettingsService;
			}
		}
		public Version Get(WebVersionResponse request) {
			return SettingsService.Version;
		}

		public Version Get(WebNewestVersionResponse request) {
			return SettingsService.NewestVersion;
		}
		
		public Version Get(WebUpdateVersionResponse request) {
			SettingsService.PerformUpdate();

			return SettingsService.NewestVersion;
		}

		public bool Get(WebRebootResponse request) {
			SettingsService.PerformReboot();

			return true;
		}
	}
}

