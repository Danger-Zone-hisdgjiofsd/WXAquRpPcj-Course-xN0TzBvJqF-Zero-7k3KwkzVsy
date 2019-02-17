using CourseZero.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Web;

namespace CourseZero.Tools
{
    public class CUSIS_Tool
    {
        public static async Task<(bool, string)> Login_To_CUSIS(HttpClient client, string username, string password)
        {
            try
            {
                var values = new Dictionary<string, string>
                {
                   { "userid", username },
                   { "pwd", password }
                };
                var content = new FormUrlEncodedContent(values);
                var response = await client.PostAsync("https://cusis.cuhk.edu.hk/psp/csprd/?cmd=login&languageCd=ENG", content);
                foreach (var cookie in response.Headers.GetValues("Set-Cookie"))
                    client.DefaultRequestHeaders.Add("Cookie", cookie);
                response = await client.GetAsync("https://cusis.cuhk.edu.hk/psp/csprd/EMPLOYEE/HRMS/h/?tab=DEFAULT");
                foreach (var cookie in response.Headers.GetValues("Set-Cookie"))
                    client.DefaultRequestHeaders.Add("Cookie", cookie);
                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(await response.Content.ReadAsStringAsync());
                var error_node = htmlDocument.DocumentNode.SelectNodes("//*[@id=\"login_error\"]");
                if (error_node != null)
                    return (false, error_node[0].InnerText);
            }
            catch (Exception ex)
            {
                return (false, ex.ToString());
            }
            return (true, "");

        }
        public static async Task<List<Course>> Scan_CoursePage(HttpClient client)
        {
            List<Course> courses = new List<Course>();
            var response = await client.GetAsync("https://cusis.cuhk.edu.hk/psc/csprd/EMPLOYEE/HRMS/c/COMMUNITY_ACCESS.SSS_BROWSE_CATLG.GBL");
            foreach (var cookie in response.Headers.GetValues("Set-Cookie"))
                client.DefaultRequestHeaders.Add("Cookie", cookie);
            char prefix = 'A';
            while (prefix <= 'Z')
            {
                await Scan_Courses_By_Prefix(client, courses, prefix);
                prefix++;
            }
            return courses;

        }
        public static async Task Scan_Courses_By_Prefix(HttpClient client, List<Course> courses, char prefix)
        {
            var values = new Dictionary<string, string>
            {
               { "ICAction", "DERIVED_SSS_BCC_SSR_ALPHANUM_"+ prefix }
            };
            var content = new FormUrlEncodedContent(values);
            var response = await client.PostAsync("https://cusis.cuhk.edu.hk/psc/csprd/EMPLOYEE/HRMS/c/COMMUNITY_ACCESS.SSS_BROWSE_CATLG.GBL", content);
            foreach (var cookie in response.Headers.GetValues("Set-Cookie"))
                client.DefaultRequestHeaders.Add("Cookie", cookie);
            values = new Dictionary<string, string>
            {
               { "ICAction", "DERIVED_SSS_BCC_SSS_EXPAND_ALL" }
            };
            content = new FormUrlEncodedContent(values);
            response = await client.PostAsync("https://cusis.cuhk.edu.hk/psc/csprd/EMPLOYEE/HRMS/c/COMMUNITY_ACCESS.SSS_BROWSE_CATLG.GBL", content);
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(await response.Content.ReadAsStringAsync());
            try
            {
                var cat_nodes = htmlDocument.DocumentNode.SelectNodes("//*[starts-with(@id,'DERIVED_SSS_BCC_GROUP_BOX_1')]");
                for (int cat_id = 0; cat_id < cat_nodes.Count; cat_id++)
                {
                    try
                    {
                        var cat_node = cat_nodes[cat_id];
                        var cat_str = HttpUtility.HtmlDecode(cat_node.InnerText).Split(" - ");
                        string course_prefix = cat_str[0];
                        string course_subject = cat_str[1];
                        var course_node = htmlDocument.DocumentNode.SelectNodes("//*[@id=\"COURSE_LIST$scroll$" + cat_id + "\"]/tr/td/table/tr/td/span/a");
                        for (int i = 0; i < course_node.Count / 2; i++)
                        {
                            string course_code = course_node[2 * i].InnerText;
                            string course_name = course_node[2 * i + 1].InnerText;
                            Course course = new Course();
                            course.Course_Code = course_code;
                            course.Course_Title = course_name;
                            course.Prefix = course_prefix;
                            course.Subject_Name = course_subject;
                            courses.Add(course);
                        }
                    }
                    catch
                    {

                    }
                }
            }
            catch
            {

            }
        }

    }
}
