using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CourseZero.Models
{
    public class SubscriptionContext : DbContext
    {
        public SubscriptionContext(DbContextOptions<SubscriptionContext> options) : base(options)
        {

        }
        public DbSet<Subscription> Subscriptions { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Subscription>().HasIndex(c => new { c.UserID });
            modelBuilder.Entity<Subscription>().HasKey(c => new { c.UserID, c.CourseID });
        }
        public async Task<List<int>> GetAllSubscriptions(int userid)
        {
            return await Subscriptions.Where(x => x.UserID == userid).Select(x=>x.CourseID).ToListAsync();
        }
    }
    public class Subscription
    {
        public int UserID { get; set; }
        public int CourseID { get; set; }

    }
}
