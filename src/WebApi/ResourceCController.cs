using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using NLog;

namespace DistributedLoggingTracing.WebApi
{
    [RoutePrefix("resourceC")]
    public class ResourceCController : ApiController
    {
        private readonly ILogger logger;
        private readonly HttpClient httpClient;

        public ResourceCController(ILogger logger, HttpClient httpClient)
        {
            this.logger = logger;
            this.httpClient = httpClient;
        }

        [Route("")]
        [HttpGet]
        public async Task<IHttpActionResult> Get()
        {
            var correlationInfo = CorrelationInfo.GetFromContext(Request.GetOwinContext());
            await httpClient.GetAsync("http://www.google.com");
            logger.Log(correlationInfo, LogLevel.Debug, $"Called Google from {nameof(ResourceCController)}");
            return Ok(nameof(ResourceCController));
        }
    }
}