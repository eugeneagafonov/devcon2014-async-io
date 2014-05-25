using System.Web.Http;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;

namespace AsyncInServer.WebAPI
{
	public class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			app.UseCors(CorsOptions.AllowAll);
			app.UseWebApi(createConfig());
		}

		private HttpConfiguration createConfig()
		{
			var config = new HttpConfiguration();

			config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}",
				new { id = RouteParameter.Optional });

			return config;
		}
	}
}