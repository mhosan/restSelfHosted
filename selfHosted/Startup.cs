using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Owin;


namespace selfHosted
{
    class Startup
    {
        public void Configuration(Owin.IAppBuilder app)
        {
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

            // Configure Web API for self-host.
            var config = new HttpConfiguration();

            config.Routes.MapHttpRoute(
              name: "DefaultApi",
              routeTemplate: "api/{controller}/{id}",
              defaults: new { id = RouteParameter.Optional }
            );
            //app.UseCors;
            app.UseWebApi(config);
        }
    }
}
