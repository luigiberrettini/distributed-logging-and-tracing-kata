using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using NLog;

namespace DistributedLoggingTracing.WebApi
{
    [RoutePrefix("resourceC")]
    public class ResourceCController : ApiController
    {
        private readonly HttpClient httpClient;

        public ResourceCController(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Route("")]
        [HttpGet]
        public async Task<IHttpActionResult> Get()
        {
            var correlationInfo = CorrelationInfo.GetFromContext(Request.GetOwinContext());
            await httpClient.GetAsync("http://www.google.com");
            Logger.Log(correlationInfo, LogLevel.Debug, $"Called Google from {nameof(ResourceCController)}");
            return Ok(nameof(ResourceCController));
        }
    }
}