using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CourseZero.Models
{
    public class WatchLaterContext : DbContext
    {
        public WatchLaterContext(DbContextOptions<WatchLaterContext> options) : base(options)
        {

        }
        public DbSet<WatchLater> watchLaters { get; set; }
        public async Task<List<int>> GetAllWatchLater(int userid, int next_20)
        {
            return await watchLaters.Where(x => x.UserID == userid).OrderByDescending(x=>x.ID).Skip(next_20*20).Take(20).Select(x=>x.FileID).ToListAsync();
        }
    }
    public class WatchLater
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int UserID { get; set; }
        public int FileID { get; set; }

    }
}
