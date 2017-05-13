using System;
using Microsoft.Owin.Hosting;

namespace DistributedLoggingTracing.WebApi
{
    public static class Program
    {
        public static void Main()
        {
            const string baseAddress = "http://localhost:9000";
            WebApp.Start<Startup>(baseAddress);
            Console.ReadLine();
        }
    }
}