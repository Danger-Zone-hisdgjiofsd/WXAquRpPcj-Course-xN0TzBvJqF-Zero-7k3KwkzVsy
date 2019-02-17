using CourseZero.Tools;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseZero.Models
{

    public class AuthTokenContext : DbContext
    {
        public AuthTokenContext(DbContextOptions<AuthTokenContext> options) : base(options)
        {
        }

        public DbSet<AuthToken> AuthTokens { get; set; }

        public async Task<int> Get_User_ID_By_Token(string token)
        {
            var token_to_id_result = await AuthTokens.FirstOrDefaultAsync(x => x.Token == token);
            if (token_to_id_result == null)
                return -1;
            return token_to_id_result.userID;
        }
    }
    public class AuthToken
    {
        [Key]
        public string Token { get; set; }
        public int userID { get; set; }
        public string Last_access_IP { get; set; }
        public string Last_access_Location { get; set; }
        public DateTime Last_access_Time { get; set; }
        public string Last_access_Device { get; set; }
        public string Last_access_Browser { get; set; }
    }
    public class AuthToken_Request
    {
        [Required]
        [StringLength(128, MinimumLength = 128)]
        public string auth_token { get; set; }
    }


}
