using System.Web.Http;
using Owin;

namespace DistributedLoggingTracing.WebApi
{
    public static class AppBuilderExtensions
    {
        public static IAppBuilder UseCorrelationInfo(this IAppBuilder app)
        {
            return app.Use<CorrelationInfoMiddleware>();
        }

        public static IAppBuilder UseSimpleInjector(this IAppBuilder app, HttpConfiguration config)
        {
            var simpleInjectorMiddleware = new SimpleInjectorMiddleware(config);
            simpleInjectorMiddleware.ConfigureContainer();
            return app.Use(simpleInjectorMiddleware.Invoke);
        }
    }
}