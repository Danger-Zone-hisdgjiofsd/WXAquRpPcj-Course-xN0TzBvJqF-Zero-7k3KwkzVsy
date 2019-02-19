using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CourseZero.Filters;
using CourseZero.Models;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CourseZero.Controllers
{
    [Route("api/[controller]")]
    public class AvatarController : Controller
    {
        static string[] AllowedFiles_Types = { ".png", ".jpg", ".jpeg", ".gif" };
        readonly AuthTokenContext authTokenContext;
        public AvatarController(AuthTokenContext authTokenContext)
        {
            this.authTokenContext = authTokenContext;
        }
        /// <summary>
        /// Keys are (first one MUST be auth_token):
        /// auth_token,
        /// file,
        /// [file has to be less than 1MB]
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(1_000_000)]
        public async Task<IActionResult> UploadAvatar()
        {
            var boundary = Request.GetMultipartBoundary();
            if (string.IsNullOrWhiteSpace(boundary))
                return BadRequest();
            var reader = new MultipartReader(boundary, Request.Body, 1000000);
            var valuesByKey = new Dictionary<string, string>();
            MultipartSection section;
            bool file_found = false;
            bool auth_found = false;
            int userID = -1;
            while ((section = await reader.ReadNextSectionAsync()) != null)
            {
                var contentDispo = section.GetContentDispositionHeader();
                if (contentDispo.IsFileDisposition() && !file_found && auth_found)
                {
                    file_found = true;
                    var fileSection = section.AsFileSection();
                    var fileName = fileSection.FileName;
                    string type = fileName.Substring(fileName.LastIndexOf('.'));
                    if (!File_Allowed(type))
                        return BadRequest();
                    using (var stream = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "/Avatars/" + userID + ".png", FileMode.Create))
                        await fileSection.FileStream.CopyToAsync(stream);
                }
                else if (contentDispo.IsFormDisposition())
                {
                    var formSection = section.AsFormDataSection();
                    var value = await formSection.GetValueAsync();
                    if (formSection.Name == "auth_token" && !auth_found && value.Length == 128)
                    {
                        userID = await authTokenContext.Get_User_ID_By_Token(value);
                        if (userID == -1)
                            return Unauthorized();
                        auth_found = true;
                    }
                }
            }
            if (file_found && auth_found)
                return Ok();
            return BadRequest();
        }
        private bool File_Allowed(string typename)
        {
            foreach (var s in AllowedFiles_Types)
                if (s == typename)
                    return true;
            return false;
        }
        /// <summary>
        /// Return Avatars/default.png if userid is invalid
        /// </summary>
        /// <param name="avatarByUserid_Request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        [Consumes("application/json")]
        [ServiceFilter(typeof(AuthRequired))]
        public IActionResult GetAvatarByUserid([FromBody]GetAvatarByUserid_Request avatarByUserid_Request)
        {
            if (System.IO.File.Exists((AppDomain.CurrentDomain.BaseDirectory + "Avatars/" + avatarByUserid_Request.user_id + ".png")))
                 return PhysicalFile(AppDomain.CurrentDomain.BaseDirectory + "Avatars/" + avatarByUserid_Request.user_id + ".png", "application/octet-stream");
            return PhysicalFile(AppDomain.CurrentDomain.BaseDirectory + "Avatars/default.png", "application/octet-stream");
        }
        public class GetAvatarByUserid_Request
        {
            [Required]
            [StringLength(128, MinimumLength = 128)]
            public string auth_token { get; set; }
            /// <summary>
            /// The avatar owner 
            /// </summary>
            [Required]
            public int user_id { get; set; }
        }
    }
}
