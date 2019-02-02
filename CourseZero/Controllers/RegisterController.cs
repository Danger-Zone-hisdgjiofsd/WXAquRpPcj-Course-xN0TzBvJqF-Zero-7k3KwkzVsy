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
        public RegisterController (UserContext userContext)
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
                response.display_message = "this username is already in use";
                return response;
            }
            if (await userContext.Users.FirstOrDefaultAsync(x => x.email == request.email) != null)
            {
                response.status_code = 1;
                response.display_message = "this email is already in use";
                return response;
            }
            User user = new User();
            var hashing_pw_result = Hashing_Tool.Hash_Password_by_Random_Salt(request.password);
            user.username = request.username;
            user.email = request.email;
            user.password_hash = hashing_pw_result.hashed_pw;
            user.password_salt = hashing_pw_result.salt;
            user.email_verified = false;
            user.email_verifying_hash = Hashing_Tool.Random_String(128);
            bool verification_mail_sent =  await Email_Sender.Send_Verification_Email(user.email, user.username, HttpUtility.UrlEncode(user.email_verifying_hash));
            if (!verification_mail_sent)
            {
                response.status_code = 1;
                response.display_message = "an error happpened when sending email, please try again later";
                return response;
            }
            await userContext.AddAsync(user);
            await userContext.SaveChangesAsync();
            response.display_message = "an verification email is sent to " + request.email;
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
            User user = await userContext.Users.FirstOrDefaultAsync(x => x.username == username && !x.email_verified && x.email_verifying_hash == hash);
            if (user == null)
                return "This verification link is not valid or no longer valid!";
            user.email_verified = true;
            await userContext.SaveChangesAsync();
            return "Email verified!";
        }
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
            if (!username.All(c => char.IsLetterOrDigit(c) || c == '-' || c == '_'))
                return (false, "username should contain only letter, digit, underscore (_) and hyphen (-)");
            return (true, null);
        }
        private (bool valid, string error_str) password_is_valid()
        {
            if (!username.All(c => char.IsLetterOrDigit(c) || is_char_special(c)))
                return (false, "password should contain only letter, digit, special characters ~!@#$%^&*_-+=` | \\(){}[]:;\"'<>,.?/");
            return (true, null);
        }
        private (bool valid, string error_str) email_is_valid()
        {
            string domain = email.Substring(10);
            string sid = email.Substring(0, 10);
            if (domain != "@link.cuhk.edu.hk")
                return (false, "only email with domain link.cuhk.edu.hk is allowed");
            if (!sid.All(c => char.IsDigit(c)))
                return (false, "invalid email");
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