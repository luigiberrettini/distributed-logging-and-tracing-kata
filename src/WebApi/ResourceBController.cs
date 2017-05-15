using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using NLog;

namespace DistributedLoggingTracing.WebApi
{
    [RoutePrefix("resourceB")]
    public class ResourceBController : ApiController
    {
        private readonly ILogger logger;
        private readonly HttpClient httpClient;

        public ResourceBController(ILogger logger, HttpClient httpClient)
        {
            this.logger = logger;
            this.httpClient = httpClient;
        }

        [Route("")]
        [HttpGet]
        public async Task<IHttpActionResult> Get()
        {
            var baseUri = new Uri(Request.RequestUri.GetLeftPart(UriPartial.Authority));
            var request = new HttpRequestMessage(HttpMethod.Get, new Uri(baseUri, "resourceC"));
            var response = await httpClient.SendAsync(request);
            var responseStatus = response.IsSuccessStatusCode ? "Success" : "Failure";
            var correlationInfo = CorrelationInfo.GetFromContext(Request.GetOwinContext());
            logger.Log(correlationInfo, LogLevel.Debug, $"{responseStatus}: called {nameof(ResourceCController)}");
            return Ok(nameof(ResourceBController));
        }
    }
}