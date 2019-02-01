using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using CourseZero.Email;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CourseZero
{
    public class Program
    {
        static Email_Sender email_Sender;
        public static void Main(string[] args)
        {
            email_Sender = new Email_Sender();
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
