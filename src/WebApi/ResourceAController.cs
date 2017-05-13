using System.Web.Http;
using NLog;

namespace DistributedLoggingTracing.WebApi
{
    [RoutePrefix("resourceA")]
    public class ResourceAController : ApiController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Route("")]
        [HttpGet]
        public IHttpActionResult Get()
        {
            Logger.Debug($"Called {nameof(ResourceAController)}");

            return Ok(nameof(ResourceAController));
        }
    }
}