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
        public async Task<UploadHist> Get_UploadHist_By_Hist_ID(int id)
        {
            return await UploadHistories.FirstOrDefaultAsync(x => x.ID == id);
        }
    }
    public class UploadHist
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        /// <summary>
        /// File title
        /// </summary>
        public string File_Name { get; set; }
        /// <summary>
        /// File extension (.docx, .pdf ...)
        /// </summary>
        public string File_typename { get; set; }
        public string File_Description { get; set; }
        public int Related_courseID { get; set; }
        public int Uploader_UserID { get; set; }
        public DateTime Upload_Time { get; set; }
        /// <summary>
        /// True if the file is processed
        /// </summary>
        public bool Processed { get; set; }
        /// <summary>
        /// True if the file is processed successfully
        /// </summary>
        public bool Processed_Success { get; set; }
        /// <summary>
        /// If processing fail, the error id returned. Check out File_Process_Tool.cs for meanings.
        /// </summary>
        public int Procesed_ErrorMsg { get; set; }
        /// <summary>
        /// If the file is processed successfully, this will be the uploaded file ID. (Maybe useful in redirection to the view page)
        /// </summary>
        public int Processed_FileID { get; set; }
    }
}
