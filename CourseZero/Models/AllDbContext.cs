using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseZero.Models
{
    public class AllDbContext : DbContext
    {
        public AllDbContext(DbContextOptions<AllDbContext> options) : base(options)
        {

        }
        public DbSet<AuthToken> AuthTokens { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<ProfileComment> ProfileComments { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<UploadedFile> UploadedFiles { get; set; }
        public DbSet<UploadHist> UploadHistories { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<WatchLater> watchLaters { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProfileComment>().HasIndex(c => new { c.receiver_UserID });
            modelBuilder.Entity<Subscription>().HasIndex(c => new { c.UserID });
            modelBuilder.Entity<Subscription>().HasKey(c => new { c.UserID, c.CourseID });
            modelBuilder.Entity<UploadedFile>().HasIndex(c => new { c.Upload_Time });
            modelBuilder.Entity<UploadedFile>().HasIndex(c => new { c.Likes });
            modelBuilder.Entity<User>().HasIndex(c => new { c.username }).IsUnique(true);
            modelBuilder.Entity<User>().HasIndex(c => new { c.email }).IsUnique(true);
        }

        public async Task<List<int>> GetAllWatchLater(int userid, int next_20)
        {
            return await watchLaters.Where(x => x.UserID == userid).OrderByDescending(x => x.ID).Skip(next_20 * 20).Take(20).Select(x => x.FileID).ToListAsync();
        }
        public async Task<User> Get_User_By_User_ID(int id)
        {
            return await Users.FirstOrDefaultAsync(x => x.ID == id);
        }
        public async Task<User> Get_User_By_Email(string email)
        {
            return await Users.FirstOrDefaultAsync(x => x.email == email);
        }
        public async Task<UploadHist> Get_UploadHist_By_Hist_ID(int id)
        {
            return await UploadHistories.FirstOrDefaultAsync(x => x.ID == id);
        }
        public async Task<UploadedFile> Get_File_By_FileID(int fileid)
        {
            return await UploadedFiles.FirstOrDefaultAsync(x => x.ID == fileid);
        }

        public async Task<List<int>> GetAllSubscriptions(int userid)
        {
            return await Subscriptions.Where(x => x.UserID == userid).Select(x => x.CourseID).ToListAsync();
        }
        public async Task<int> Get_User_ID_By_Token(string token)
        {
            var token_to_id_result = await AuthTokens.FirstOrDefaultAsync(x => x.Token == token);
            if (token_to_id_result == null)
                return -1;
            return token_to_id_result.userID;
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
}
