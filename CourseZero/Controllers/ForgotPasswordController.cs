using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using CourseZero.Email;
using CourseZero.Hashing;
using CourseZero.Models;
using Microsoft.AspNetCore.Mvc;


namespace CourseZero.Controllers
{
    [Route("api/[controller]")]
    public class ForgotPasswordController : Controller
    {
        readonly AllDbContext allDbContext;
        public ForgotPasswordController(AllDbContext allDbContext)
        {
            this.allDbContext = allDbContext;
        }

        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [Route("[action]")]
        public async Task<ActionResult<ForgotPassword_Change_Response>> Request_Change([FromBody]ForgotPassword_Change_Request request)
        {
            var response = new ForgotPassword_Change_Response();
            User user = await allDbContext.Get_User_By_Email(request.email);
            if (user == null)
            {
                response.status_code = 1;
                response.display_message = "Account does not exist";
                return response;
            }
            if (!user.email_verified)
            {
                response.status_code = 1;
                response.display_message = "Account has not been verified";
                return response;
            }
            if (DateTime.Compare(user.password_change_request_datatime.AddHours(24), DateTime.Now) > 0)
            {
                response.status_code = 1;
                response.display_message = "You can only issue a password change request every 24 hours";
                return response;
            }
            string new_password = Random_Password();
            string hash = Hashing_Tool.Random_String(128);
            if (!await Email_Sender.Send_Password_Change_Email(user.email, user.ID, user.username, new_password, HttpUtility.UrlEncode(hash)))
            {
                response.status_code = 1;
                response.display_message = "An error happpened when sending email, please try again later";
                return response;
            }
            user.password_change_new_password = new_password;
            user.password_change_hash = hash;
            user.password_change_request_datatime = DateTime.Now;
            await allDbContext.SaveChangesAsync();
            response.status_code = 0;
            response.display_message = "A new password is sent to your email. Please activate your new password within 30 minutes.";
            return response;
        }
        [HttpGet]
        [Route("[action]/{id}/{hash}")]
        public async Task<ActionResult<string>> Activate(int id, string hash)
        {
            hash = HttpUtility.UrlDecode(hash).Replace(' ', '+');
            if (hash.Length != 128)
                return "This activation link is not valid!";
            User user = await allDbContext.Get_User_By_User_ID(id);
            if (user == null || user.password_change_hash != hash || DateTime.Compare(user.password_change_request_datatime.AddMinutes(30), DateTime.Now) < 0)
                return "This activation link is not valid or no longer valid!";
            var hashing_pw_result = Hashing_Tool.Hash_Password_by_Random_Salt(user.password_change_new_password);
            user.password_hash = hashing_pw_result.hashed_pw;
            user.password_salt = hashing_pw_result.salt;
            user.password_change_hash = "";
            user.password_change_new_password = "";
            var sessions =  allDbContext.AuthTokens.Where(x => x.userID == id);
            allDbContext.AuthTokens.RemoveRange(sessions);
            await allDbContext.SaveChangesAsync();
            return "Your new password has been activated.";
        }

        static string Random_Password(int length = 8)
        {
            Random random = new Random();
            string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789~!@#$%^&*_-+=`|\\(){}[]:;\"'<>,.?/";
            string password = "";
            while (length > 0)
            {
                password += chars[random.Next(chars.Length)];
                length--;
            }
            return password;
        }
        public class ForgotPassword_Change_Request
        {
            /// <summary>
            /// A new password and an activation link will be sent to the user email
            /// </summary>
            [Required]
            [StringLength(27, MinimumLength = 27)]
            public string email { get; set; }
        }
        public class ForgotPassword_Change_Response
        {
            /// <summary>
            ///  <para>0 is success, 1 is fail</para>
            ///  <para>all current sessions will be logged out if success</para>
            /// </summary>
            public int status_code { get; set; }
            public string display_message { get; set; }
        }
    }
}
