using Owin;

namespace DistributedLoggingTracing.WebApi
{
    public static class AppBuilderExtensions
    {
        public static IAppBuilder UseCorrelationInfo(this IAppBuilder app)
        {
            return app.Use<CorrelationInfoMiddleware>();
        }
    }
}