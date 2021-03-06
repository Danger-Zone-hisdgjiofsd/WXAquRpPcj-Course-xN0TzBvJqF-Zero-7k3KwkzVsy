﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CourseZero.Hashing;
using CourseZero.Models;
using CourseZero.Tools;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CourseZero.Controllers
{
    [Route("api/[controller]")]
    public class LoginController : Controller
    {
        readonly AllDbContext allDbContext;
        public LoginController(AllDbContext allDbContext)
        {
            this.allDbContext = allDbContext;
        }
        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<ActionResult<Login_Response>> Post([FromBody]Login_Request request)
        {
            var response = new Login_Response();
            if (request.email == null && request.username == null)
            {
                response.status_code = 1;
                return response;
            }
            var valid_check = request.all_fields_are_valid();
            if (!valid_check.valid)
            {
                response.status_code = 1;
                response.display_message = valid_check.error_str;
                return response;
            }
            User user = null;
            if (request.using_email)
                user = await allDbContext.Users.FirstOrDefaultAsync(x => x.email == request.email);
            else
                user = await allDbContext.Users.FirstOrDefaultAsync(x => x.username == request.username);
            if (user == null)
            {
                response.status_code = 1;
                response.display_message = "User not found";
                return response;
            }
            if (!user.email_verified)
            {
                response.status_code = 1;
                response.display_message = "Please verify your email first";
                return response;
            }
            string hashed_password = Hashing_Tool.Hash_Password(request.password, user.password_salt);
            if (hashed_password != user.password_hash)
            {
                response.status_code = 1;
                response.display_message = "Wrong password";
                return response;
            }
            //Create Login Token Here
            string token = "";
            while (true)
            {
                token = Hashing_Tool.Random_String(128).ToLower();
                if (await allDbContext.AuthTokens.FirstOrDefaultAsync(x => x.Token == token) != null)
                    continue;
                break;
            }
            AuthToken auth_Token_Obj;
            var existing_tokens = allDbContext.AuthTokens.Where(x => x.userID == user.ID).OrderByDescending(x => x.Last_access_Time);
            if (existing_tokens.Count() == 20) // Max token per user is 20
                allDbContext.AuthTokens.Remove(existing_tokens.Last());

            auth_Token_Obj = new AuthToken();

            auth_Token_Obj.Token = token;
            auth_Token_Obj.userID = user.ID;
            string useragent_str = "";
            if (Request.Headers.ContainsKey("User-Agent"))
                useragent_str = Request.Headers["User-Agent"].ToString();
            await RequestSource_Tool.Update_AuthToken_Browse_Record(auth_Token_Obj, useragent_str, Request.HttpContext.Connection.RemoteIpAddress.ToString());
            await allDbContext.AddAsync(auth_Token_Obj);
            await allDbContext.SaveChangesAsync();

            response.username = user.username;
            response.auth_token = token;
            response.status_code = 0;
            response.display_message = "success";
            return response;
        }
        public class Login_Response
        {
            /// <summary>
            /// 0 is success, 1 is fail
            /// </summary>
            public int status_code { get; set; }
            public string display_message { get; set; }
            /// <summary>
            /// <para>A hash that serve as a credential for later purposes </para>
            /// <para>Should be stored in localstorage / sessionstorage (if the user does not check the "remember me" option)</para>
            /// </summary>
            public string auth_token { get; set; }
            public string username { get; set; }
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
                    return (false, "Password should contain only letter, digit, special characters ~!@#$%^&*_-+=`|\\(){}[]:;\"'<>,.?/");
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
                string specials = "~!@#$%^&*_-+=`|\\(){}[]:;\"'<>,.?/";
                foreach (var s in specials)
                {
                    if (c == s)
                        return true;
                }
                return false;
            }
        }
    }
}
