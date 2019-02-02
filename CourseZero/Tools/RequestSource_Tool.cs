using CourseZero.Models;
using DeviceDetectorNET;
using DeviceDetectorNET.Cache;
using System;
using System.Collections.Generic;
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
                return await wc.DownloadStringTaskAsync(new Uri("https://ipapi.co/"+ip+"/country_name"));
            }
            catch
            {
                return "";
            }
        }
    }
}
