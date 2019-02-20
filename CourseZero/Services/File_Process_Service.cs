using CourseZero.Models;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CourseZero.Services
{
    public class File_Process_Service : BackgroundService
    {
        public static Queue<UploadHist> Process_Queue = new Queue<UploadHist>();
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            
        }
    }
}
