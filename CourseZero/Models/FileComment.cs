using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CourseZero.Models
{
    public class FileComment
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int sender_UserID { get; set; }
        public int file_ID { get; set; }
        [MaxLength(2048)]
        public string Text { get; set; }
        public DateTime posted_dateTime { get; set; }
    }
    public class FileComment_ShownToUser
    {
        public int ID { get; set; }
        public int sender_UserID { get; set; }
        public string Text { get; set; }
        public DateTime posted_dateTime { get; set; }
        public string sender_Username { get; set; }

    }
}

