namespace DistributedLoggingTracing.WebApi
{
    public class TraceInfo
    {
        public ICorrelationInfo CorrelationInfo { get; }

        public HttpRequestDetails ParentCallInfo { get; }

        public HttpRequestDetails CallInfo { get; }

        public TraceInfo(ICorrelationInfo correlationInfo, HttpRequestDetails callInfo)
            : this(correlationInfo, HttpRequestDetails.Empty, callInfo)
        {
        }

        public TraceInfo(ICorrelationInfo correlationInfo, HttpRequestDetails parentCallInfo, HttpRequestDetails callInfo)
        {
            CorrelationInfo = correlationInfo;
            ParentCallInfo = parentCallInfo;
            CallInfo = callInfo;
        }
    }
}