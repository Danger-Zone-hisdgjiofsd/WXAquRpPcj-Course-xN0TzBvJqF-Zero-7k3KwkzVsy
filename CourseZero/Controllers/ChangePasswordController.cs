using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CourseZero.Hashing;
using CourseZero.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Z.Linq;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CourseZero.Controllers
{
    [Route("api/[controller]")]
    public class ChangePasswordController : Controller
    {
        // GET: /<controller>/
        readonly UserContext userContext;
        readonly AuthTokenContext authTokenContext;
        public ChangePasswordController(UserContext userContext, AuthTokenContext authTokenContext)
        {
            this.userContext = userContext;
            this.authTokenContext = authTokenContext;
        }
        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<ActionResult<Password_Change_Response>> Post([FromBody] Password_Change_Request request)
        {
            var response = new Password_Change_Response();
            var password_verify_result = request.password_is_valid(request.old_password);
            if (!password_verify_result.valid)
            {
                response.status_code = 2;
                response.display_message = password_verify_result.error_str;
                return response;
            }
            password_verify_result = request.password_is_valid(request.new_password);
            if (!password_verify_result.valid)
            {
                response.status_code = 3;
                response.display_message = password_verify_result.error_str;
                return response;
            }
            AuthToken token = await authTokenContext.AuthTokens.FirstOrDefaultAsync(x => x.Token == request.auth_token);
            if (token == null)
            {
                response.status_code = 1;
                return response;
            }
            User user = await userContext.Get_User_By_User_ID(token.userID);
            string hashed_password = Hashing_Tool.Hash_Password(request.old_password, user.password_salt);
            if (hashed_password != user.password_hash)
            {
                response.status_code = 2;
                response.display_message = "wrong password";
                return response;
            }
            var hashing_pw_result = Hashing_Tool.Hash_Password_by_Random_Salt(request.new_password);
            user.password_hash = hashing_pw_result.hashed_pw;
            user.password_salt = hashing_pw_result.salt;
            var sessions = await authTokenContext.AuthTokens.WhereAsync(x => x.userID == token.userID);
            authTokenContext.AuthTokens.RemoveRange(sessions);
            await userContext.SaveChangesAsync();
            await authTokenContext.SaveChangesAsync();
            response.status_code = 0;
            response.display_message = "Your password has been changed. Please login again.";
            return response; 

        }
        public class Password_Change_Request
        {
            [Required]
            [StringLength(128, MinimumLength = 128)]
            public string auth_token { get; set; }
            [Required]
            [StringLength(20, MinimumLength = 5)]
            public string old_password { get; set; }
            [Required]
            [StringLength(20, MinimumLength = 5)]
            public string new_password { get; set; }
            public (bool valid, string error_str) password_is_valid(string password)
            {
                if (!password.All(c => char.IsLetterOrDigit(c) || is_char_special(c)))
                    return (false, "password should contain only letter, digit, special characters ~!@#$%^&*_-+=` | \\(){}[]:;\"'<>,.?/");
                return (true, null);
            }

        }
        public class Password_Change_Response
        {
            /// <summary>
            /// <para>0 is success, 1, 2, 3 is fail</para>
            /// <para>1 fail is due to invalid auth token</para>
            /// <para>2 fail is due to invalid old password</para>
            /// <para>3 fail is due to invalid new password</para>
            /// <para>all current sessions will be logged out if success</para>
            /// </summary>
            public int status_code { get; set; }
            public string display_message { get; set; }

        }
        private static bool is_char_special(char c)
        {
            string specials = "~!@#$%^&*_-+=` | \\(){}[]:;\"'<>,.?/";
            foreach (var s in specials)
            {
                if (c == s)
                    return true;
            }
            return false;
        }
    }
}
