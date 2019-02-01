using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CourseZero.Hashing;
using CourseZero.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CourseZero.Controllers
{
    [Route("api/[controller]")]
    public class LoginController : Controller
    {
        readonly UserContext userContext;
        public LoginController(UserContext userContext)
        {
            this.userContext = userContext;
        }
        [HttpPost]
        [Produces("application/json")]
        public async Task<ActionResult<Login_Response>> Post([FromBody]Login_Request request)
        {
            var response = new Login_Response();
            var valid_check = request.all_fields_are_valid();
            if (!valid_check.valid)
            {
                response.status_code = 1;
                response.display_message = valid_check.error_str;
                return response;
            }
            User user = null;
            if (request.using_email)
                user = await userContext.Users.FirstOrDefaultAsync(x => x.email == request.email);
            else
                user = await userContext.Users.FirstOrDefaultAsync(x => x.username == request.username);
            if (user == null)
            {
                response.status_code = 1;
                response.display_message = "user not found";
                return response;
            }
            if (!user.email_verified)
            {
                response.status_code = 1;
                response.display_message = "please verify your email first";
                return response;
            }
            string hashed_password = Hashing_Tool.Hash_Password(request.password, user.password_salt);
            if (hashed_password != user.password_hash)
            {
                response.status_code = 1;
                response.display_message = "wrong password";
                return response;
            }
            response.status_code = 0;
            response.display_message = "success";
            return response;
        }
    }
    public class Login_Request
    {
        /// <summary>
        /// Should be TRUE if the user is trying to login with @link email. Otherwise, an username is expected.
        /// </summary>
        [Required]
        public bool using_email { get; set; }
        [StringLength(20, MinimumLength = 5)]
        public string username { get; set; }
        [StringLength(27, MinimumLength = 27)]
        public string email { get; set; }
        [Required]
        [StringLength(20, MinimumLength = 5)]
        public string password { get; set; }
        [Required]
        public string recaptcha_hash { get; set; }

        public (bool valid, string error_str) all_fields_are_valid()
        {

            var result = password_is_valid();
            if (!result.valid)
                return result;
            if (using_email)
            {
                result = email_is_valid();
                if (!result.valid)
                    return result;
            }
            else
            {
                result = username_is_valid();
                if (!result.valid)
                    return result;
            }
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
    public class Login_Response
    {
        /// <summary>
        /// 0 is success, 1 is fail
        /// </summary>
        public int status_code { get; set; }
        public string display_message { get; set; }
    }
}
