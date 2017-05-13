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
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Route("")]
        [HttpGet]
        public async Task<IHttpActionResult> Get()
        {
            var correlationInfo = CorrelationInfo.GetFromContext(Request.GetOwinContext());

            using (var httpClient = new HttpClient(new CorrelationInfoMessageHandler(Logger, Request, correlationInfo)))
            {
                httpClient.BaseAddress = new Uri(Request.RequestUri.GetLeftPart(UriPartial.Authority));
                var response = await httpClient.GetAsync("/resourceC");
                var responseStatus = response.IsSuccessStatusCode ? "Success" : "Failure";
                Logger.Log(correlationInfo, LogLevel.Debug, $"{responseStatus}: called {nameof(ResourceCController)}");
                return Ok(nameof(ResourceBController));
            }
        }
    }
}