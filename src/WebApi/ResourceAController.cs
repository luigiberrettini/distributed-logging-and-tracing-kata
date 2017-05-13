using System.Net.Http;
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
            Logger.Log(Request.GetOwinContext(), LogLevel.Debug, $"Called {nameof(ResourceAController)}");
            return Ok(nameof(ResourceAController));
        }
    }
}