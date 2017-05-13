using System;
using Microsoft.Owin.Hosting;
using NLog;

namespace DistributedLoggingTracing.WebApi
{
    public static class Program
    {
        private const string BaseAddress = "http://localhost:9000";
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static void Main()
        {
            Logger.Debug("Starting application");
            WebApp.Start<Startup>(BaseAddress);
            Console.ReadLine();
        }
    }
}