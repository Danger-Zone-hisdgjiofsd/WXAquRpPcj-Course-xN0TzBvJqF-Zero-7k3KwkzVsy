﻿using CourseZero.Models;
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
        static string Courses_Json_Str = "";
        static bool Courses_Json_Str_Lazy_added = false;
        bool Initial = true;
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
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var CourseContext = scope.ServiceProvider.GetService<CourseContext>();
                foreach (var course in CourseContext.Courses)
                {
                    Courses.Add(course.Prefix+course.Course_Code, course);
                }
            }
            while (true)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                if (!(Initial && Courses.Count() == 0))
                    await Task.Delay(86400000); //24 hours
                Initial = false;
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.AllowAutoRedirect = false;
                HttpClient client = new HttpClient(clientHandler);
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/72.0.3626.109 Safari/537.36");
                var login_success = await CUSIS_Tool.Login_To_CUSIS(client, CUSIS_ACCOUNT, CUSIS_PASSWORD);
                if (!login_success.Item1)
                {
                    Console.WriteLine("ERROR WHEN TRYING TO FETCH CUSIS: " + login_success.Item2);
                    await Task.Delay(86400000); //24 hours
                    continue;
                }
                Console.WriteLine("FETCHING CUSIS ...");
                int orginal_count = Courses.Count();

                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var course_list = await CUSIS_Tool.Scan_CoursePage(client);
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
                client = null;
                clientHandler = null;
                Courses.Clear();
                Courses_Json_Str_Lazy_added = false;
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var CourseContext = scope.ServiceProvider.GetService<CourseContext>();
                    foreach (var course in CourseContext.Courses)
                    {
                        Courses.Add(course.Prefix + course.Course_Code, course);
                    }
                }
                Get_Course_Json_Str();
                Console.WriteLine("FETCHING CUSIS ... DONE, ADDED " + (Courses.Count() - orginal_count).ToString() + " COURSES");
            }
        }

}
}