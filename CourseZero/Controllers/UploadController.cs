using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CourseZero.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CourseZero.Controllers
{
    [Route("api/[controller]")]
    public class UploadController : Controller
    {
        static string[] AllowedFiles_Types = {".txt", ".doc", ".docx", ".ppt", ".pptx", ".pdf", ".wav", ".mp3", ".3gp", ".mp4", ".avi", ".mkv"};
        readonly AuthTokenContext authTokenContext;
        readonly UploadHistContext uploadHistContext;
        public UploadController(AuthTokenContext authTokenContext, UploadHistContext uploadHistContext)
        {
            this.authTokenContext = authTokenContext;
            this.uploadHistContext = uploadHistContext;
        }
        /// <summary>
        /// Keys are (first one MUST be auth_token):
        /// auth_token,
        /// file_name,
        /// file_description,
        /// file,
        /// [file has to be less than 100MB]
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(100_000_000)]
        public async Task<IActionResult> UploadFile()
        {
            var boundary = Request.GetMultipartBoundary();
            if (string.IsNullOrWhiteSpace(boundary))
                return BadRequest();
            var reader = new MultipartReader(boundary, Request.Body, 80 * 1024);
            var valuesByKey = new Dictionary<string, string>();
            MultipartSection section;
            bool file_found = false;
            bool auth_found = false;
            string file_name = "";
            string file_description = "";
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
                    if (file_name == "")
                        file_name = fileSection.FileName;
                    UploadHist uploadHist = new UploadHist();
                    uploadHist.Uploader_UserID = userID;
                    uploadHist.Upload_Time = DateTime.Now;
                    uploadHist.Processed = false;
                    uploadHist.File_Name = file_name;
                    uploadHist.File_typename = type.Substring(1);
                    await uploadHistContext.AddAsync(uploadHist);
                    await uploadHistContext.SaveChangesAsync();
                    using (var stream = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "/UploadsQueue/" + uploadHist.ID + type, FileMode.CreateNew))
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
                    if (formSection.Name == "file_name")
                        file_name = value;
                    if (formSection.Name == "file_description")
                        file_description = value;
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

    }
}
