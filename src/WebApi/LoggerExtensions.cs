using NLog;

namespace DistributedLoggingTracing.WebApi
{
    public static class LoggerExtensions
    {
        public static void Log(this ILogger logger, ICorrelationInfo correlationInfo, LogLevel logLevel, string message)
        {
            var logEventInfo = new LogEventInfo(logLevel, "", message);

            logEventInfo.Properties["requestId"] = correlationInfo.RequestId;
            logEventInfo.Properties["parentCallId"] = correlationInfo.ParentCallId;
            logEventInfo.Properties["callId"] = correlationInfo.CallId;

            logger.Log(logEventInfo);
        }
    }
}