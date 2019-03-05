using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CourseZero.Models
{
    public enum ReportType
    {
        File,
        User,
        File_Comment,
        Profile_Comment,
        General
    }
    public class Report
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int UserID { get; set; }
        public int RelatedID { get; set; }
        public int Report_Type { get; set; }
        [MaxLength(10240)]
        public string Text { get; set; }
        public bool Resovled { get; set; }
        public DateTime ReportTime { get; set; }
    }
}
