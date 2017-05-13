using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using NLog;

namespace DistributedLoggingTracing.WebApi
{
    [RoutePrefix("resourceC")]
    public class ResourceCController : ApiController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Route("")]
        [HttpGet]
        public async Task<IHttpActionResult> Get()
        {
            var correlationInfo = CorrelationInfo.GetFromContext(Request.GetOwinContext());

            using (var httpClient = new HttpClient())
            {
                await httpClient.GetAsync("http://www.google.com");
                Logger.Log(correlationInfo, LogLevel.Debug, $"Called Google from {nameof(ResourceCController)}");
                return Ok(nameof(ResourceCController));
            }
        }
    }
}