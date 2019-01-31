using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace CourseZero.Controllers
{
    [Route("api/[controller]")]

    public class RegisterController : Controller
    {
        [HttpPost]
        [Produces("application/json")]
        public Register_Response Post([FromBody]Register_Request request)
        {
            var response = new Register_Response();
            var valid_check = request.all_fields_are_valid();
            if (!valid_check.valid)
            {
                response.status_code = 1;
                response.display_message = valid_check.error_str;
                return response;
            }
            response.display_message = "Good";
            response.status_code = 0;
            return response;
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
        /// success = 0, fail = 1
        /// </summary>
        public int status_code { get; set; }
        public string display_message { get; set; }
    }
}