﻿using CourseZero.Models;
using CourseZero.Tools;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static CourseZero.Tools.File_Process_Tool;

namespace CourseZero.Services
{
    public class File_Process_Service : BackgroundService
    {
        const double MAX_PROCESS_MIN = 3;
        public static Queue<UploadHist> Process_Queue = new Queue<UploadHist>();
        public static Dictionary<int, (UploadHist uploadhist, int queue)> Queue_Position = new Dictionary<int, (UploadHist uploadhist, int queue)>();
        readonly IServiceScopeFactory serviceScopeFactory;
        Task<FileProcess_Status> Task_FileProcessing = null;
        Thread Processing_Thread = null;
        DateTime Process_StartTime;
        public static UploadHist Current_ProcessObj;
        public File_Process_Service(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }
        public static void Enqueue(UploadHist item)
        {
            Process_Queue.Enqueue(item);
            Queue_Position.Add(item.ID, (item, Process_Queue.Count));
        }
        private void Queue_Position_Minus_One()
        {
            var list_of_key = Queue_Position.Keys.ToList();
            foreach (var item in list_of_key)
            {
                var obj = Queue_Position[item];
                Queue_Position[item] = (obj.uploadhist, obj.queue - 1);
            }
        }
        private async Task Add_Incompleted_Job_FromDB()
        {
            int count = 0;
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var allDbContext = scope.ServiceProvider.GetService<AllDbContext>();
                var incompleted_jobs = allDbContext.UploadHistories.Where(x => !x.Processed);
                foreach (var job in incompleted_jobs)
                {
                    count++;
                    Enqueue(job);
                }
            }
            Console.WriteLine("FILE_PROCESS_SERVICE: ADDED " + count);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Add_Incompleted_Job_FromDB();
            while (true)
            {
                if (Task_FileProcessing != null)
                {
                    if (Task_FileProcessing.IsCompleted || Task_FileProcessing.IsCanceled || Task_FileProcessing.IsFaulted)
                    {
                        Console.WriteLine("FILE_PROCESS_SERVICE: COMPLETED - " + Current_ProcessObj.ID + " " + Task_FileProcessing.Result);
                        Task_FileProcessing = null;
                    }
                    else if (DateTime.Compare(DateTime.Now, Process_StartTime.AddMinutes(MAX_PROCESS_MIN)) > 0)
                    {
                        Console.WriteLine("FILE_PROCESS_SERVICE: " + Current_ProcessObj.ID + " " + "TIME OUT!");
                        Processing_Thread.Abort();
                        Task_FileProcessing = null;
                        using (var scope = serviceScopeFactory.CreateScope())
                        {
                            var uploadHistContext = scope.ServiceProvider.GetService<AllDbContext>();
                            var hist = await uploadHistContext.UploadHistories.FirstOrDefaultAsync(x => x.ID == Current_ProcessObj.ID);
                            hist.Processed = true;
                            hist.Processed_Success = false;
                            hist.Procesed_ErrorMsg = (int)FileProcess_Status.TimeOut;
                            await uploadHistContext.SaveChangesAsync();
                            uploadHistContext.Dispose();
                        }
                    }
                }
                else if (Process_Queue.Count > 0 && Task_FileProcessing == null)
                {
                    Queue_Position_Minus_One();
                    UploadHist To_Process = Process_Queue.Dequeue();
                    Queue_Position.Remove(To_Process.ID);
                    Current_ProcessObj = To_Process;
                    Console.WriteLine("FILE_PROCESS_SERVICE: PROCESSING " + Current_ProcessObj.ID);
                    Task_FileProcessing = Task.Factory.StartNew(() =>
                        {
                            Processing_Thread = Thread.CurrentThread;
                            Try_Close_AllHandles();
                            return Process_File(To_Process);
                        } 
                        , TaskCreationOptions.LongRunning);
                    Process_StartTime = DateTime.Now;
                }
                await Task.Delay(5000);
            }
        }
        private FileProcess_Status Process_File(UploadHist ItemtoProcess)
        {
            string physical_path = AppDomain.CurrentDomain.BaseDirectory + "/UploadsQueue/" + ItemtoProcess.ID +  ItemtoProcess.File_typename;
            if (!File.Exists(physical_path))
            {
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var uploadHistContext = scope.ServiceProvider.GetService<AllDbContext>();
                    var hist = uploadHistContext.UploadHistories.FirstOrDefault(x => x.ID == ItemtoProcess.ID);
                    hist.Processed = true;
                    hist.Processed_Success = false;
                    hist.Procesed_ErrorMsg = (int)FileProcess_Status.NotFound;
                    uploadHistContext.SaveChanges();
                    uploadHistContext.Dispose();
                }
                return FileProcess_Status.NotFound;
            }
            var processed_result = Process(physical_path, ItemtoProcess.File_typename);
            if (processed_result.status != FileProcess_Status.Success)
            {
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var uploadHistContext = scope.ServiceProvider.GetService<AllDbContext>();
                    var hist = uploadHistContext.UploadHistories.FirstOrDefault(x => x.ID == ItemtoProcess.ID);
                    hist.Processed = true;
                    hist.Processed_Success = false;
                    hist.Procesed_ErrorMsg = (int) processed_result.status;
                    uploadHistContext.SaveChanges();
                    uploadHistContext.Dispose();
                }
                return processed_result.status;
            }
            EncoderParameters myEncoderParameters = new EncoderParameters(1);
            EncoderParameter myEncoderParameter = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 1L);
            myEncoderParameters.Param[0] = myEncoderParameter;
            int FileID = -1;
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var allDbContext = scope.ServiceProvider.GetService<AllDbContext>();
                var hist = allDbContext.UploadHistories.FirstOrDefault(x => x.ID == ItemtoProcess.ID);
                var Course = CUSIS_Fetch_Service.GetCourse_By_CourseID(hist.Related_courseID);
                StringBuilder stringBuider = new StringBuilder();
                stringBuider.Append(Course.Prefix + " ");
                stringBuider.Append(Course.Course_Code + " ");
                stringBuider.Append(Course.Course_Title + " ");
                stringBuider.Append(Course.Subject_Name + " ");
                stringBuider.Append(hist.File_Description + " ");
                stringBuider.Append(hist.File_Name + " ");

                UploadedFile uploadedFile = new UploadedFile
                {
                    Course_Prefix = Course.Prefix,
                    File_Description = hist.File_Description,
                    File_Name = hist.File_Name,
                    File_Typename = hist.File_typename,
                    Uploader_UserID = hist.Uploader_UserID,
                    Upload_Time = hist.Upload_Time,
                    Related_courseID = hist.Related_courseID,
                    Words_for_Search = stringBuider.ToString()
                };
                if (processed_result.content != null) //Store it internally
                {
                    uploadedFile.Stored_Internally = true;
                    uploadedFile.Binary = processed_result.content;
                }
                else
                    uploadedFile.Stored_Internally = false;
                allDbContext.UploadedFiles.Add(uploadedFile);
                allDbContext.SaveChanges();
                FileID = uploadedFile.ID;
                hist.Processed = true;
                hist.Processed_Success = true;
                hist.Processed_FileID = uploadedFile.ID;
                allDbContext.SaveChanges();
                allDbContext.Dispose();
            }
            processed_result.thumbnail.Save(AppDomain.CurrentDomain.BaseDirectory + "/UploadsThumbnail/" + FileID + ".png", GetEncoder(ImageFormat.Jpeg), myEncoderParameters);
            Try_Close_AllHandles();
            if (processed_result.content == null)
                File.Move(physical_path, AppDomain.CurrentDomain.BaseDirectory + "/Uploads/" + FileID + ItemtoProcess.File_typename);
            else
                File.Delete(physical_path);
            return FileProcess_Status.Success;
        }
    }
}
