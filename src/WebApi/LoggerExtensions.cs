using Microsoft.Owin;
using NLog;

namespace DistributedLoggingTracing.WebApi
{
    public static class LoggerExtensions
    {
        public static void Log(this Logger logger, IOwinContext context, LogLevel logLevel, string message)
        {
            var logEventInfo = new LogEventInfo(logLevel, "", message);

            var correlationInfo = CorrelationInfo.GetFromContext(context);
            logEventInfo.Properties["requestId"] = correlationInfo.RequestId;
            logEventInfo.Properties["parentCallId"] = correlationInfo.ParentCallId;
            logEventInfo.Properties["callId"] = correlationInfo.CallId;

            logger.Log(logEventInfo);
        }
    }
}