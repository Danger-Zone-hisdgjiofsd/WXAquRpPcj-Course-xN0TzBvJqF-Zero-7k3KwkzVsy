using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CourseZero.Models
{
    public class ProfileCommentsContext : DbContext
    {
        public ProfileCommentsContext(DbContextOptions<ProfileCommentsContext> options) : base(options)
        {

        }
        public DbSet<ProfileComment> ProfileComments { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProfileComment>().HasIndex(c => new { c.receiver_UserID });
        }
        public async Task<List<ProfileComment>> GetComments(int userid, int next_20)
        {
            return await ProfileComments.Where(x => x.receiver_UserID == userid).OrderByDescending(x => x.ID).Skip(next_20 * 20).Take(20).ToListAsync();
        }
        public async Task<ProfileComment> GetCommentByID(int commentid)
        {
            return await ProfileComments.FirstOrDefaultAsync(x => x.ID == commentid);
        }
    }
    public class ProfileComment
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int sender_UserID { get; set; }
        public int receiver_UserID { get; set; }
        [MaxLength(2048)]
        public string Text { get; set; }
        public DateTime posted_dateTime { get; set; }

    }
}
