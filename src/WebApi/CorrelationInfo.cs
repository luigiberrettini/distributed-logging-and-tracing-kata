using System;
using Microsoft.Owin;

namespace DistributedLoggingTracing.WebApi
{
    public class CorrelationInfo : ICorrelationInfo
    {
        public const string RequestIdHeaderName = "DLT-CI-RequestId";
        public const string ParentCallIdHeaderName = "DLT-CI-ParentCallId";
        public const string CallIdHeaderName = "DLT-CI-CallId";
        private const string ContextEnvironmentKey = "DistributedLoggingTracing.CorrelationInfo";
        private const string IdForMissingParentCall = "0";

        public string RequestId { get; }

        public string ParentCallId { get; }

        public string CallId { get; }

        public static void SetFromHeaders(IOwinContext context = null)
        {
            var correlationInfo = new CorrelationInfo(context);
            context?.Environment.Add(ContextEnvironmentKey, correlationInfo);
        }

        public static ICorrelationInfo GetFromContext(IOwinContext context = null)
        {
            object contextInfo;
            context.Environment.TryGetValue(ContextEnvironmentKey, out contextInfo);
            return (ICorrelationInfo)contextInfo;
        }

        public ICorrelationInfo ToInfoForOutgoingRequest()
        {
            return new CorrelationInfo(RequestId, CallId, GenerateNewId());
        }

        private CorrelationInfo(IOwinContext context)
        {
            var headers = context?.Request?.Headers;

            var headerRequestId = headers?[RequestIdHeaderName];
            var headerParentCallId = headers?[ParentCallIdHeaderName];
            var headerCallId = headers?[CallIdHeaderName];

            RequestId = IdFromHeaderOrDefault(headerRequestId, GenerateNewId());
            ParentCallId = IdFromHeaderOrDefault(headerParentCallId, IdForMissingParentCall);
            CallId = IdFromHeaderOrDefault(headerCallId, GenerateNewId());
        }

        private CorrelationInfo(string requestId, string parentCallId, string callId)
        {
            RequestId = requestId;
            ParentCallId = parentCallId;
            CallId = callId;
        }

        private string GenerateNewId()
        {
            return Guid.NewGuid().ToString("N");
        }

        private string IdFromHeaderOrDefault(string header, string defaultValue)
        {
            Guid parsedGuid;
            return Guid.TryParseExact(header, "N", out parsedGuid) ? header : defaultValue;
        }
    }
}