using System.Net.Http;
using System.Threading.Tasks;
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
        public async Task<IHttpActionResult> Get()
        {
            var correlationInfo = CorrelationInfo.GetFromContext(Request.GetOwinContext());

            using (var httpClient = new HttpClient(new CorrelationInfoMessageHandler(Logger, correlationInfo)))
            {
                var response = await httpClient.GetAsync("http://www.google.com");

                var responseStatus = response.IsSuccessStatusCode ? "Success" : "Failure";
                Logger.Log(correlationInfo, LogLevel.Debug, $"{responseStatus} calling {nameof(ResourceAController)}");

                return Ok(nameof(ResourceAController));
            }
        }
    }
}