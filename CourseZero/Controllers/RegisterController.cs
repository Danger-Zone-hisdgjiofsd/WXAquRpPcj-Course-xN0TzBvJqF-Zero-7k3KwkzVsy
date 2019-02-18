using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using CourseZero.Email;
using CourseZero.Hashing;
using CourseZero.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseZero.Controllers
{
    [Route("api/[controller]")]

    public class RegisterController : Controller
    {
        readonly UserContext userContext;
        public RegisterController(UserContext userContext)
        {
            this.userContext = userContext;
        }
        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<ActionResult<Register_Response>> Post([FromBody]Register_Request request)
        {
            var response = new Register_Response();
            var valid_check = request.all_fields_are_valid();
            if (!valid_check.valid)
            {
                response.status_code = 1;
                response.display_message = valid_check.error_str;
                return response;
            }
            if (await userContext.Users.FirstOrDefaultAsync(x => x.username == request.username) != null)
            {
                response.status_code = 1;
                response.display_message = "This username is already in use";
                return response;
            }
            if (await userContext.Users.FirstOrDefaultAsync(x => x.email == request.email) != null)
            {
                response.status_code = 1;
                response.display_message = "This email is already in use";
                return response;
            }
            User user = new User();
            var hashing_pw_result = Hashing_Tool.Hash_Password_by_Random_Salt(request.password);
            user.username = request.username;
            user.email = request.email;
            user.password_hash = hashing_pw_result.hashed_pw;
            user.password_salt = hashing_pw_result.salt;
            user.email_verified = false;
            string hash = Hashing_Tool.Random_String(128);
            bool verification_mail_sent = await Email_Sender.Send_Verification_Email(user.email, user.username, HttpUtility.UrlEncode(hash));
            if (!verification_mail_sent)
            {
                response.status_code = 1;
                response.display_message = "An error happpened when sending email, please try again later";
                return response;
            }
            user.email_verifying_hash = hash;
            user.email_verification_issue_datetime = DateTime.Now;
            await userContext.AddAsync(user);
            await userContext.SaveChangesAsync();
            response.display_message = "An verification email is sent to " + request.email + ", please verify your account within 2 hours";
            response.status_code = 0;
            return response;
        }
        /// <summary>
        /// Reissue an verification email
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [Route("[action]")]
        public async Task<ActionResult<Reissue_Email_Response>> Reissue_Email([FromBody]Reissue_Email_Request request)
        {
            var response = new Reissue_Email_Response();
            User user = await userContext.Get_User_By_Email(request.email);
            if (user == null)
            {
                response.status_code = 1;
                response.display_message = "Account does not exist";
                return response;
            }
            if (user.email_verified)
            {
                response.status_code = 1;
                response.display_message = "This account has already been verified";
                return response;
            }
            if (DateTime.Compare(user.email_verification_issue_datetime.AddHours(12), DateTime.Now) > 0)
            {
                response.status_code = 1;
                response.display_message = "You can only request a verification email every 12 hours";
                return response;
            }
            string hash = Hashing_Tool.Random_String(128);
            bool verification_mail_sent = await Email_Sender.Send_Verification_Email(user.email, user.username, HttpUtility.UrlEncode(hash));
            if (!verification_mail_sent)
            {
                response.status_code = 1;
                response.display_message = "An error happpened when sending email, please try again later";
                return response;
            }
            user.email_verifying_hash = hash;
            user.email_verification_issue_datetime = DateTime.Now;
            await userContext.SaveChangesAsync();
            response.display_message = "An verification email is sent to " + request.email + ", please verify your account within 2 hours";
            response.status_code = 0;
            return response;
        }

        [HttpGet]
        [Route("[action]/{username}/{hash}")]
        public async Task<ActionResult<string>> Verify_Email(string username, string hash)
        {
            hash = HttpUtility.UrlDecode(hash).Replace(' ', '+');
            if (username.Length > 20 || hash.Length != 128)
                return "This verification link is not valid!";
            User user = await userContext.Users.FirstOrDefaultAsync(x => x.username == username);
            if (user == null || user.email_verified || user.email_verifying_hash != hash || DateTime.Compare(user.email_verification_issue_datetime.AddHours(2), DateTime.Now) < 0)
                return "This verification link is not valid or no longer valid!";
            user.email_verified = true;
            await userContext.SaveChangesAsync();
            return "Email verified!";
        }

        public class Reissue_Email_Request
        {
            [Required]
            [StringLength(27, MinimumLength = 27)]
            public string email { get; set; }
        }
        public class Reissue_Email_Response
        {
            /// <summary>
            ///  0 is success, 1 is fail
            /// </summary>
            public int status_code { get; set; }
            public string display_message { get; set; }
        }
        public class Register_Request
        {
            [Required]
            [StringLength(20, MinimumLength = 5)]
            public string username { get; set; }
            [Required]
            [StringLength(20, MinimumLength = 5)]
            public string password { get; set; }
            [Required]
            [StringLength(27, MinimumLength = 27)]
            public string email { get; set; }
            [Required]
            public string recaptcha_hash { get; set; }

            public (bool valid, string error_str) all_fields_are_valid()
            {
                var result = username_is_valid();
                if (!result.valid)
                    return result;
                result = password_is_valid();
                if (!result.valid)
                    return result;
                result = email_is_valid();
                if (!result.valid)
                    return result;
                result = recaptcha_hash_is_valid();
                if (!result.valid)
                    return result;
                return (true, null);
            }
            private (bool valid, string error_str) username_is_valid()
            {
                int letterdigit_count = 0;
                foreach (var c in username)
                {
                    if (char.IsLetterOrDigit(c))
                        letterdigit_count++;
                    else if (c != '-' && c != '_')
                        return (false, "Username should contain only letter, digit, underscore (_) and hyphen (-)");
                }
                if (letterdigit_count < 3)
                    return (false, "Username should contain at least 3 letters or digits");
                return (true, null);
            }
            private (bool valid, string error_str) password_is_valid()
            {
                if (!password.All(c => char.IsLetterOrDigit(c) || is_char_special(c)))
                    return (false, "Password should contain only letter, digit, special characters ~!@#$%^&*_-+=` | \\(){}[]:;\"'<>,.?/");
                return (true, null);
            }
            private (bool valid, string error_str) email_is_valid()
            {
                string domain = email.Substring(10);
                string sid = email.Substring(0, 10);
                if (domain != "@link.cuhk.edu.hk")
                    return (false, "Only email with domain link.cuhk.edu.hk is allowed");
                if (!sid.All(c => char.IsDigit(c)))
                    return (false, "Invalid email");
                return (true, null);
            }
            private (bool valid, string error_str) recaptcha_hash_is_valid()
            {
                //To be done
                return (true, null);
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
        public class Register_Response
        {
            /// <summary>
            /// 0 is success, 1 is fail
            /// </summary>
            public int status_code { get; set; }
            public string display_message { get; set; }
        }
    }
}