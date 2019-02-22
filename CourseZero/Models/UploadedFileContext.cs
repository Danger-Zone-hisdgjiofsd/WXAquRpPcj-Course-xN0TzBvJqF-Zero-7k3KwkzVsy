using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CourseZero.Models
{
    public class UploadedFileContext : DbContext
    {
        public UploadedFileContext(DbContextOptions<UploadedFileContext> options) : base(options)
        {

        }
        public DbSet<UploadedFile> UploadedFiles { get; set; }
    }
    public class UploadedFile
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [MaxLength(256)]
        public string File_Name { get; set; }
        [MaxLength(10)]
        public string File_Typename { get; set; }
        [MaxLength(10240)]
        public string File_Description { get; set; }
        public int Related_courseID { get; set; }
        public string Course_Prefix { get; set; }
        public int Uploader_UserID { get; set; }
        public DateTime Upload_Time { get; set; }
        public int Likes { get; set; }
        public int DisLikes { get; set; }
        public string Words_for_Search { get; set; }
        public bool Stored_Internally { get; set; }
        public byte[] Binary { get; set; }
    }
    public class File_Shown_to_User
    {
        public int File_ID { get; set; }
        public string File_Name { get; set; }
        public string File_Typename { get; set; }
        public string File_Description { get; set; }
        public int Related_courseID { get; set; }
        public int Uploader_UserID { get; set; }
        public DateTime Upload_Time { get; set; }
        public int Likes { get; set; }
        public int DisLikes { get; set; }

    }
}
