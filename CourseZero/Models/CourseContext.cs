using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CourseZero.Models
{
    public class CourseContext : DbContext
    {
        public CourseContext(DbContextOptions<CourseContext> options) : base(options)
        {
        }
        public DbSet<Course> Courses { get; set; }
    }
    public class Course
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string Prefix { get; set; }
        public string Subject_Name { get; set; }
        public string Course_Code { get; set; }
        public string Course_Title { get; set; }
    }
}
