using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Owin;
using NLog;
using SimpleInjector;
using SimpleInjector.Integration.WebApi;
using SimpleInjector.Lifestyles;

namespace DistributedLoggingTracing.WebApi
{
    public sealed class SimpleInjectorMiddleware : IDisposable
    {
        private readonly Container container;

        public SimpleInjectorMiddleware(HttpConfiguration config)
        {
            container = new Container();
            var simpleInjectorDependencyResolver = new SimpleInjectorWebApiDependencyResolver(container);
            config.DependencyResolver = simpleInjectorDependencyResolver;
        }

        public void ConfigureContainer()
        {
            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            container.Register(() => new HttpClient(), Lifestyle.Singleton);

            container.Register(() => new Func<ILogger, HttpRequestMessage, DelegatingHandler>(
                (logger, request) => new CorrelationInfoMessageHandler(logger, request)),
                Lifestyle.Singleton);
        }

        public async Task Invoke(IOwinContext context, Func<Task> next)
        {
            using (AsyncScopedLifestyle.BeginScope(container))
            {
                await next();
            }
        }

        public void Dispose()
        {
            container.Dispose();
        }
    }
}