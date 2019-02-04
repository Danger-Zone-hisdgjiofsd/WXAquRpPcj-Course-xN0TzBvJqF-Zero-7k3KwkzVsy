using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CourseZero.Models
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public async Task<User> Get_User_By_User_ID(int id)
        {
            return await Users.FirstOrDefaultAsync(x => x.ID == id);
        }
        public async Task<User> Get_User_By_Email(string email)
        {
            return await Users.FirstOrDefaultAsync(x => x.email == email);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(c => new { c.username, c.email }).IsUnique(true);
        }
    }
    public class User
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public string password_hash { get; set; }
        public string password_salt { get; set; }
        public bool email_verified { get; set; }
        public string email_verifying_hash { get; set; }
        public DateTime email_verification_issue_datetime { get; set; }
        public string password_change_new_password { get; set; }
        public string password_change_hash { get; set; }
        public DateTime password_change_request_datatime { get; set; }

    }
}
