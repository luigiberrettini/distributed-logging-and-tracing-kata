using System.Web.Http;
using Owin;

namespace DistributedLoggingTracing.WebApi
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            var config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();
            appBuilder
                .UseCorrelationInfo()
                .UseWebApi(config);
        }
    }
}