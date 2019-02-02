using CourseZero.Models;
using DeviceDetectorNET;
using DeviceDetectorNET.Cache;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CourseZero.Tools
{
    public class RequestSource_Tool
    {
        public static DeviceDetector deviceDetector = new DeviceDetector();
        static WebClient wc = new WebClient();
        public static (string operation_system, string browser) Decode_UA(string ua)
        {
            deviceDetector.SetUserAgent(ua);
            deviceDetector.Parse();
            string operation_system = "unknown";
            string browser = "unknown";
            var os_info = deviceDetector.GetOs();
            if (os_info.Success)
                operation_system = os_info.Match.Name + " " + os_info.Match.Platform + " " + os_info.Match.Version;
            var browser_info = deviceDetector.GetBrowserClient();
            if (browser_info.Success)
                browser = browser_info.Match.Name + " " + browser_info.Match.Version;
            return (operation_system, browser);
        }
        public async static Task<string> IP_to_Location(string ip)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://ipapi.co/" + ip + "/country_name");
                request.Timeout = 2000;
                request.ReadWriteTimeout = 2000;
                var response = await request.GetResponseAsync();
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream);
                    return reader.ReadToEnd();
                }
            }
            catch
            {
                return "";
            }
        }
    }
}
