using System.Threading.Tasks;
using Microsoft.Owin;

namespace DistributedLoggingTracing.WebApi
{
    public class CorrelationInfoMiddleware : OwinMiddleware
    {
        public CorrelationInfoMiddleware(OwinMiddleware next) : base(next)
        {
        }

        public override Task Invoke(IOwinContext context)
        {
            CorrelationInfo.SetFromHeaders(context);
            return Next.Invoke(context);
        }
    }
}