using System.Web.Http;
using Owin;

namespace DistributedLoggingTracing.WebApi
{
    public static class AppBuilderExtensions
    {
        public static IAppBuilder UseSimpleInjector(this IAppBuilder app, HttpConfiguration config)
        {
            var simpleInjectorMiddleware = new SimpleInjectorMiddleware(config);
            simpleInjectorMiddleware.ConfigureContainer();
            return app.Use(simpleInjectorMiddleware.Invoke);
        }

        public static IAppBuilder UseOwinCallContext(this IAppBuilder app)
        {
            return app.Use<OwinCallContextMiddleware>();
        }

        public static IAppBuilder UseCorrelationInfo(this IAppBuilder app)
        {
            return app.Use<CorrelationInfoMiddleware>();
        }
    }
}