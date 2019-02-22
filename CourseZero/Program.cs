using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using CourseZero.Email;
using CourseZero.Tools;
using DeviceDetectorNET.Cache;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CourseZero
{
    public class Program
    {
        static Email_Sender email_Sender;
        public static string db_Connection_Str = "";
        public static void Main(string[] args)
        {
            email_Sender = new Email_Sender();
            //Setting up device detector cache
            RequestSource_Tool.deviceDetector.SetCache(new DictionaryCache());     
            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "/Avatars/")) //Place for user avatar
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "/Avatars/");
            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "/UploadsQueue/")) //Place for pending file
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "/UploadsQueue/");
            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "/temp/")) //Temp place for file processing
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "/temp/");
            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "/UploadsThumbnail/")) //Place for file thumbnail
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "/UploadsThumbnail/");
            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "/Uploads/")) //Place for file storage
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "/Uploads/");
            Console.WriteLine("Start");
            var host = CreateWebHostBuilder(args).Build();
            Console.WriteLine("Run webhost");
            host.Run();
            Console.WriteLine("End webhost");
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args).ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.SetBasePath(AppDomain.CurrentDomain.BaseDirectory).AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);
            }).UseStartup<Startup>();
    }
}
