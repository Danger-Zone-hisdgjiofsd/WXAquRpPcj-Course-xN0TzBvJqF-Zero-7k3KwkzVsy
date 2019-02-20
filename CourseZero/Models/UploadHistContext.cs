using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CourseZero.Models
{
    public class UploadHistContext : DbContext
    {
        public UploadHistContext(DbContextOptions<UploadHistContext> options) : base(options)
        {

        }
        public DbSet<UploadHist> UploadHistories { get; set; }
    }
    public class UploadHist
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string File_Name { get; set; }
        public string File_typename { get; set; }
        public int Related_courseID { get; set; }
        public int Uploader_UserID { get; set; }
        public DateTime Upload_Time { get; set; }
        public bool Processed { get; set; }
        public bool Processed_Success { get; set; }
        public int Processed_FileID { get; set; }
    }
}
