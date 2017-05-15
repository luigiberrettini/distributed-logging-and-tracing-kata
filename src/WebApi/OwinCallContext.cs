using System.Runtime.Remoting.Messaging;
using Microsoft.Owin;

namespace DistributedLoggingTracing.WebApi
{
    public static class OwinCallContext
    {
        private const string OwinContextKey = "owin.IOwinContext";

        public static IOwinContext Current => (IOwinContext)CallContext.LogicalGetData(OwinContextKey);

        public static void Set(IOwinContext context)
        {
            CallContext.LogicalSetData(OwinContextKey, context);
        }

        public static void Remove()
        {
            CallContext.FreeNamedDataSlot(OwinContextKey);
        }
    }
}