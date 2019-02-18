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
        static Dictionary<string, string> ip_to_location_cache = new Dictionary<string, string>();
        const int cache_limit = 10000;
        public static (string operation_system, string browser) Decode_UA(string ua)
        {
            if (ua == "")
                return ("", "");
            deviceDetector.SetUserAgent(ua);
            deviceDetector.Parse();
            string operation_system = "unknown";
            string browser = "unknown";
            var os_info = deviceDetector.GetOs();
            if (os_info.Success)
                operation_system = os_info.Match.Name + " " + os_info.Match.Version + " " + os_info.Match.Platform;
            var browser_info = deviceDetector.GetBrowserClient();
            if (browser_info.Success)
                browser = browser_info.Match.Name + " " + browser_info.Match.Version;

            if (deviceDetector.GetCache().Count() > cache_limit)
                deviceDetector.GetCache().FlushAll();
            return (operation_system, browser);
        }
        public async static Task<string> IP_to_Location(string ip)
        {
            if (ip_to_location_cache.ContainsKey(ip))
                return ip_to_location_cache[ip];
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://ipapi.co/" + ip + "/country_name");
                request.Timeout = 2000;
                request.ReadWriteTimeout = 2000;
                var response = await request.GetResponseAsync();
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream);
                    string loc = reader.ReadToEnd();
                    if (ip_to_location_cache.Count > cache_limit)
                        ip_to_location_cache.Clear();
                    ip_to_location_cache.Add(ip, loc);
                    return loc;
                }
            }
            catch
            {
                return "";
            }
        }
        public static async Task Update_AuthToken_Browse_Record(AuthToken auth_Token_Obj, string ua, string ip)
        {

            var ua_decoded = Decode_UA(ua);
            string loc = await IP_to_Location(ip);
            auth_Token_Obj.Last_access_Browser = ua_decoded.browser;
            auth_Token_Obj.Last_access_Device = ua_decoded.operation_system;
            auth_Token_Obj.Last_access_Location = loc;
            auth_Token_Obj.Last_access_IP = ip;
            auth_Token_Obj.Last_access_Time = DateTime.Now;
        }
    }
}
