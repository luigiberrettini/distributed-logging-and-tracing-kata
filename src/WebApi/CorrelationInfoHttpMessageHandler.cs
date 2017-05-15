using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using NLog;

namespace DistributedLoggingTracing.WebApi
{
    public class CorrelationInfoHttpMessageHandler : DelegatingHandler
    {
        private readonly Stopwatch stopwatch;
        private readonly ILogger logger;

        public CorrelationInfoHttpMessageHandler(ILogger logger)
            : base(new HttpClientHandler())
        {
            stopwatch = new Stopwatch();
            this.logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var parentCorrelationInfo = CorrelationInfo.GetFromContext(OwinCallContext.Current);
            var correlationInfo = parentCorrelationInfo.ToInfoForOutgoingRequest();
            request.Headers.Add(CorrelationInfo.RequestIdHeaderName, correlationInfo.RequestId);
            request.Headers.Add(CorrelationInfo.ParentCallIdHeaderName, correlationInfo.ParentCallId);
            request.Headers.Add(CorrelationInfo.CallIdHeaderName, correlationInfo.CallId);

            stopwatch.Start();
            var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
            stopwatch.Stop();
            logger.Trace(BuildTraceInfo(correlationInfo, request, stopwatch.ElapsedMilliseconds), "Performed outgoing call");
            return response;
        }

        private static TraceInfo BuildTraceInfo(ICorrelationInfo correlationInfo, HttpRequestMessage request, long duration)
        {
            var parentRequest = OwinCallContext.Current.Request;
            var parentCallInfo = new HttpRequestDetails(parentRequest.Uri, parentRequest.Method);
            var callInfo = new HttpRequestDetails(request.RequestUri, request.Method.Method, duration);
            return new TraceInfo(correlationInfo, parentCallInfo, callInfo);
        }
    }
}