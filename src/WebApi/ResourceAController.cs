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
            var correlationInfo = CorrelationInfo.GetFromContext(Request.GetOwinContext());
            Logger.Log(correlationInfo, LogLevel.Debug, $"This is {nameof(ResourceAController)}");
            return Ok(nameof(ResourceAController));
        }
    }
}