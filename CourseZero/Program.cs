using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CourseZero
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Start");
            var host = CreateWebHostBuilder(args).Build();
            Console.WriteLine("Run webhost");
            host.Run();
            Console.WriteLine("End webhost");
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
