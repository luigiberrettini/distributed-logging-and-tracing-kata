using System;
using NLog;

namespace DistributedLoggingTracing.WebApi
{
    public static class LoggerExtensions
    {
        public static void Log(this ILogger logger, ICorrelationInfo correlationInfo, LogLevel logLevel, string message)
        {
            var logEventInfo = new LogEventInfo(logLevel, "", message);
            logEventInfo.FillWithTimestamp();
            logEventInfo.FillWithCorrelationInfo(correlationInfo);
            logger.Log(logEventInfo);
        }

        public static void Trace(this ILogger logger, TraceInfo traceInfo, string message)
        {
            var logEventInfo = new LogEventInfo(LogLevel.Trace, "", message);
            logEventInfo.FillWithTimestamp();
            logEventInfo.FillWithCorrelationInfo(traceInfo.CorrelationInfo);
            logEventInfo.FillWithParentCallInfo(traceInfo.ParentCallInfo);
            logEventInfo.FillWithCallInfo(traceInfo.CallInfo);
            logger.Log(logEventInfo);

        }

        private static void FillWithTimestamp(this LogEventInfo logEventInfo)
        {
            logEventInfo.Properties["occurredOn"] = DateTime.UtcNow.ToString("O");
        }

        private static void FillWithCorrelationInfo(this LogEventInfo logEventInfo, ICorrelationInfo correlationInfo)
        {
            logEventInfo.Properties["requestId"] = correlationInfo.RequestId;
            logEventInfo.Properties["parentCallId"] = correlationInfo.ParentCallId;
            logEventInfo.Properties["callId"] = correlationInfo.CallId;
        }

        private static void FillWithParentCallInfo(this LogEventInfo logEventInfo, HttpRequestDetails parentCallInfo)
        {
            if (parentCallInfo == HttpRequestDetails.Empty)
                return;

            logEventInfo.Properties["parentCallUri"] = parentCallInfo.Uri;
            logEventInfo.Properties["parentCallMethod"] = parentCallInfo.Method;
        }

        private static void FillWithCallInfo(this LogEventInfo logEventInfo, HttpRequestDetails callInfo)
        {
            logEventInfo.Properties["callUri"] = callInfo.Uri;
            logEventInfo.Properties["callMethod"] = callInfo.Method;
            logEventInfo.Properties["callDuration"] = callInfo.Duration;
        }
    }
}