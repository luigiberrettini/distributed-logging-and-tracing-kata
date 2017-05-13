using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using NLog;

namespace DistributedLoggingTracing.WebApi
{
    public class CorrelationInfoMessageHandler : DelegatingHandler
    {
        private readonly Stopwatch stopwatch;
        private readonly ILogger logger;
        private readonly ICorrelationInfo parentCorrelationInfo;

        public CorrelationInfoMessageHandler(ILogger logger, ICorrelationInfo parentCorrelationInfo)
            : this(logger, parentCorrelationInfo, new HttpClientHandler())
        {
        }

        public CorrelationInfoMessageHandler(ILogger logger, ICorrelationInfo parentCorrelationInfo, HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
            stopwatch = new Stopwatch();
            this.logger = logger;
            this.parentCorrelationInfo = parentCorrelationInfo;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var correlationInfo = parentCorrelationInfo.ToInfoForOutgoingRequest();

            request.Headers.Add(CorrelationInfo.RequestIdHeaderName, correlationInfo.RequestId);
            request.Headers.Add(CorrelationInfo.ParentCallIdHeaderName, correlationInfo.ParentCallId);
            request.Headers.Add(CorrelationInfo.CallIdHeaderName, correlationInfo.CallId);


            stopwatch.Restart();
            var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
            stopwatch.Stop();
            logger.Log(correlationInfo, LogLevel.Debug, stopwatch.ElapsedMilliseconds.ToString());
            return response;
        }
    }
}