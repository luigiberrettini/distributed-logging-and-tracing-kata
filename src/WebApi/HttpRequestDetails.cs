using System;

namespace DistributedLoggingTracing.WebApi
{
    public class HttpRequestDetails
    {
        public static HttpRequestDetails Empty { get; } = new HttpRequestDetails();

        public string Uri { get; }

        public string Method { get; }

        public long Duration { get; }

        public HttpRequestDetails(Uri uri, string method, long duration = -1)
        {
            Uri = uri.AbsoluteUri;
            Method = method;
            Duration = duration;
        }

        private HttpRequestDetails()
        {
        }
    }
}