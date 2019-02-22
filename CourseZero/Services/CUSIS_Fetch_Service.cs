using CourseZero.Models;
using CourseZero.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CourseZero.Services
{
    public class CUSIS_Fetch_Service : BackgroundService
    {
        readonly IServiceScopeFactory serviceScopeFactory;
        static string CUSIS_ACCOUNT = "";
        static string CUSIS_PASSWORD = "";
        public CUSIS_Fetch_Service(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
            try
            {
                string[] strs = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "/cusis_settings.cfg");
                CUSIS_ACCOUNT = strs[0];
                CUSIS_PASSWORD = strs[1];
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error when reading cusis settings!");
                Console.WriteLine(ex.ToString());
                Console.WriteLine("Press any key to exit");
                Console.ReadLine();
                Environment.Exit(0);
            }
        }
        static Dictionary<string, Course> Courses = new Dictionary<string, Course>();
        static Dictionary<int, Course> Courses_FromID = new Dictionary<int, Course>();
        static string Courses_Json_Str = "";
        static bool Courses_Json_Str_Lazy_added = false;
        public static (int lower, int upper) CourseID_range = (int.MaxValue, int.MinValue);
        bool Initial = true;
        public static Course GetCourse_By_CourseID(int id)
        {
            if (Courses_FromID.ContainsKey(id))
                return Courses_FromID[id];
            return null;
        }
        public static string Get_Course_Json_Str()
        {
            if (!Courses_Json_Str_Lazy_added)
            {
                Courses_Json_Str = JsonConvert.SerializeObject(Courses.Values); 
                Courses_Json_Str_Lazy_added = true;
            }
            return Courses_Json_Str;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Courses.Clear();
            Courses_FromID.Clear();
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var CourseContext = scope.ServiceProvider.GetService<CourseContext>();
                foreach (var course in CourseContext.Courses)
                {
                    Courses.Add(course.Prefix+course.Course_Code, course);
                    Courses_FromID.Add(course.ID, course);
                    if (CourseID_range.lower > course.ID)
                        CourseID_range.lower = course.ID;
                    if (CourseID_range.upper < course.ID)
                        CourseID_range.upper = course.ID;
                }
            }
            while (true)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                if (!(Initial && Courses.Count() == 0))
                    await Task.Delay(86400000); //24 hours
                Initial = false;
                CUSIS_Tool cusis_Tool = new CUSIS_Tool();
                var login_success = await cusis_Tool.Login_To_CUSIS(CUSIS_ACCOUNT, CUSIS_PASSWORD);
                if (!login_success.Item1)
                {
                    Console.WriteLine("ERROR WHEN TRYING TO FETCH CUSIS: " + login_success.Item2);
                    continue;
                }
                Console.WriteLine("FETCHING CUSIS ...");
                int orginal_count = Courses.Count();

                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var course_list = await cusis_Tool.Scan_CoursePage();
                    List<Course> courses_to_add = new List<Course>();
                    var courseContext = scope.ServiceProvider.GetService<CourseContext>();
                    foreach (var scanned_course in course_list)
                    {
                        string key = scanned_course.Prefix + scanned_course.Course_Code;
                        if (Courses.ContainsKey(key))
                        {
                            if (Courses[key].Course_Title == scanned_course.Course_Title)
                                continue;
                        }
                        courses_to_add.Add(scanned_course);
                    }
                    await courseContext.Courses.AddRangeAsync(courses_to_add);
                    await courseContext.SaveChangesAsync();
                    course_list.Clear();
                    courses_to_add.Clear();
                    courseContext.Dispose();
                }
                Courses.Clear();
                Courses_FromID.Clear();
                Courses_Json_Str_Lazy_added = false;
                CourseID_range = (int.MaxValue, int.MinValue);
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var CourseContext = scope.ServiceProvider.GetService<CourseContext>();
                    foreach (var course in CourseContext.Courses)
                    {
                        Courses.Add(course.Prefix + course.Course_Code, course);
                        Courses_FromID.Add(course.ID, course);
                        if (CourseID_range.lower > course.ID)
                            CourseID_range.lower = course.ID;
                        if (CourseID_range.upper < course.ID)
                            CourseID_range.upper = course.ID;
                    }
                }
                Get_Course_Json_Str();
                Console.WriteLine("FETCHING CUSIS ... DONE, ADDED " + (Courses.Count() - orginal_count).ToString() + " COURSES");
            }
        }

}
}
