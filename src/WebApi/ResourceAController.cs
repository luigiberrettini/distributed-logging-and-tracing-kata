using System.Net.Http;
using System.Web.Http;
using NLog;

namespace DistributedLoggingTracing.WebApi
{
    [RoutePrefix("resourceA")]
    public class ResourceAController : ApiController
    {
        private readonly ILogger logger;

        public ResourceAController(ILogger logger)
        {
            this.logger = logger;
        }

        [Route("")]
        [HttpGet]
        public IHttpActionResult Get()
        {
            var correlationInfo = CorrelationInfo.GetFromContext(Request.GetOwinContext());
            logger.Log(correlationInfo, LogLevel.Debug, $"This is {nameof(ResourceAController)}");
            return Ok(nameof(ResourceAController));
        }
    }
}