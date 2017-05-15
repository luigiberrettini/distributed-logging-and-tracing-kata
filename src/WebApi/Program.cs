using System;
using Microsoft.Owin.Hosting;

namespace DistributedLoggingTracing.WebApi
{
    public static class Program
    {
        private const string BaseAddress = "http://localhost:9000";

        public static void Main()
        {
            WebApp.Start<Startup>(BaseAddress);
            Console.ReadLine();
        }
    }
}