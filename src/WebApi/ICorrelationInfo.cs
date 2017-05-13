namespace DistributedLoggingTracing.WebApi
{
    public interface ICorrelationInfo
    {
        string RequestId { get; }

        string ParentCallId { get; }

        string CallId { get; }
    }
}