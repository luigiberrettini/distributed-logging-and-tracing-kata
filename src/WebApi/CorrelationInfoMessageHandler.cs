﻿using System.Diagnostics;
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
        private readonly HttpRequestMessage parentRequest;
        private readonly ICorrelationInfo parentCorrelationInfo;

        public CorrelationInfoMessageHandler(ILogger logger, HttpRequestMessage parentRequest, ICorrelationInfo parentCorrelationInfo)
            : this(logger, parentRequest, parentCorrelationInfo, new HttpClientHandler())
        {
            this.parentRequest = parentRequest;
        }

        public CorrelationInfoMessageHandler(ILogger logger, HttpRequestMessage parentRequest, ICorrelationInfo parentCorrelationInfo, HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
            stopwatch = new Stopwatch();
            this.logger = logger;
            this.parentRequest = parentRequest;
            this.parentCorrelationInfo = parentCorrelationInfo;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
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

        private TraceInfo BuildTraceInfo(ICorrelationInfo correlationInfo, HttpRequestMessage request, long duration)
        {
            var parentCallInfo = new HttpRequestDetails(parentRequest.RequestUri, parentRequest.Method.Method);
            var callInfo = new HttpRequestDetails(request.RequestUri, request.Method.Method, duration);
            return new TraceInfo(correlationInfo, parentCallInfo, callInfo);
        }
    }
}