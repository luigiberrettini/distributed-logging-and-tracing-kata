using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Owin;
using NLog;

namespace DistributedLoggingTracing.WebApi
{
    public class CorrelationInfoMiddleware : OwinMiddleware
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly Stopwatch stopwatch;

        public CorrelationInfoMiddleware(OwinMiddleware next) : base(next)
        {
            stopwatch = new Stopwatch();
        }

        public override async Task Invoke(IOwinContext context)
        {
            CorrelationInfo.SetFromHeaders(context);
            stopwatch.Restart();
            await Next.Invoke(context);
            stopwatch.Stop();
            Logger.Trace(BuildTraceInfo(context, stopwatch.ElapsedMilliseconds), "Handled external call");
        }

        private TraceInfo BuildTraceInfo(IOwinContext context, long duration)
        {
            var correlationInfo = CorrelationInfo.GetFromContext(context);
            var request = context.Request;
            var callInfo = new HttpRequestDetails(request.Uri, request.Method, duration);
            return new TraceInfo(correlationInfo, callInfo);
        }
    }
}