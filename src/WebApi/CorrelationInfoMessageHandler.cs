using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DistributedLoggingTracing.WebApi
{
    public class CorrelationInfoMessageHandler : DelegatingHandler
    {
        private readonly ICorrelationInfo parentCorrelationInfo;

        public CorrelationInfoMessageHandler(ICorrelationInfo parentCorrelationInfo) : this(parentCorrelationInfo, new HttpClientHandler())
        {
        }

        public CorrelationInfoMessageHandler(ICorrelationInfo parentCorrelationInfo, HttpMessageHandler innerHandler) : base(innerHandler)
        {
            this.parentCorrelationInfo = parentCorrelationInfo;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var correlationInfo = parentCorrelationInfo.ToInfoForOutgoingRequest();

            request.Headers.Add(CorrelationInfo.RequestIdHeaderName, correlationInfo.RequestId);
            request.Headers.Add(CorrelationInfo.ParentCallIdHeaderName, correlationInfo.ParentCallId);
            request.Headers.Add(CorrelationInfo.CallIdHeaderName, correlationInfo.CallId);

            return base.SendAsync(request, cancellationToken);
        }
    }
}