using System.Threading.Tasks;
using Microsoft.Owin;

namespace DistributedLoggingTracing.WebApi
{
    public class OwinCallContextMiddleware : OwinMiddleware
    {
        public OwinCallContextMiddleware(OwinMiddleware next) : base(next)
        {
        }

        public override async Task Invoke(IOwinContext context)
        {
            try
            {
                OwinCallContext.Set(context);
                await Next.Invoke(context);
            }
            finally
            {
                OwinCallContext.Remove();
            }
        }
    }
}