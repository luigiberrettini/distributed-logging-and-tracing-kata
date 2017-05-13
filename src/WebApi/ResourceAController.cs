using System.Web.Http;

namespace DistributedLoggingTracing.WebApi
{
    [RoutePrefix("resourceA")]
    public class ResourceAController : ApiController
    {
        [Route("")]
        [HttpGet]
        public IHttpActionResult Get()
        {
            return Ok(nameof(ResourceAController));
        }
    }
}