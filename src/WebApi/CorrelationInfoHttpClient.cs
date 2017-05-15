using System.Net.Http;

namespace DistributedLoggingTracing.WebApi
{
    public class CorrelationInfoHttpClient : HttpClient
    {
        public CorrelationInfoHttpClient(HttpMessageHandler handler) : base(handler)
        {
        }
    }
}