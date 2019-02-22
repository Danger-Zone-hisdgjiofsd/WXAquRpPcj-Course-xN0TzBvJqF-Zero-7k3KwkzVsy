using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CourseZero.Models;
using CourseZero.Services;
using CourseZero.Tools;
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
        /// related_courseid,
        /// file,
        /// [file has to be less than 100MB]
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        [Consumes("multipart/form-data")]
        [Produces("application/json")]
        [RequestSizeLimit(100_000_000)]
        public async Task<UploadFile_Response> UploadFile()
        {
            var boundary = Request.GetMultipartBoundary();
            if (string.IsNullOrWhiteSpace(boundary))
                return new UploadFile_Response(2);
            var reader = new MultipartReader(boundary, Request.Body, 80 * 1024);
            var valuesByKey = new Dictionary<string, string>();
            MultipartSection section;
            bool file_found = false;
            bool auth_found = false;
            int courseID = -1;
            string file_name = "";
            string file_description = "";
            int userID = -1;
            UploadHist uploadHist = new UploadHist();
            while ((section = await reader.ReadNextSectionAsync()) != null)
            {
                var contentDispo = section.GetContentDispositionHeader();
                if (contentDispo.IsFileDisposition() && !file_found && auth_found && courseID != -1)
                {
                    file_found = true;
                    var fileSection = section.AsFileSection();
                    var fileName = fileSection.FileName;
                    int dot_index = fileName.LastIndexOf('.');
                    if (dot_index < 0)
                        return new UploadFile_Response(3);
                    string type = fileName.Substring(dot_index);
                    if (!File_Process_Tool.File_Allowed(type))
                        return new UploadFile_Response(3);
                    if (file_name == "")
                        file_name = fileSection.FileName;
                    file_name = file_name.Substring(0, dot_index);
                    uploadHist.Uploader_UserID = userID;
                    uploadHist.Upload_Time = DateTime.Now;
                    uploadHist.Processed = false;
                    uploadHist.File_Name = file_name;
                    uploadHist.File_typename = type;
                    uploadHist.Related_courseID = courseID;
                    uploadHist.File_Description = file_description;
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
                            return new UploadFile_Response(1);
                        auth_found = true;
                    }
                    if (formSection.Name == "file_name")
                        file_name = value;
                    if (formSection.Name == "file_description")
                        file_description = value;
                    if (formSection.Name == "related_courseid")
                    {
                        courseID = int.TryParse(value, out courseID) ? courseID : -1;
                        if (CUSIS_Fetch_Service.CourseID_range.lower > courseID || courseID > CUSIS_Fetch_Service.CourseID_range.upper)
                            return new UploadFile_Response(4);
                    }
                }
            }
            if (file_found && auth_found && courseID != -1)
            {
                File_Process_Service.Process_Queue.Enqueue(uploadHist);
                return new UploadFile_Response(0, uploadHist.ID);
            }
            return new UploadFile_Response(2);
        }
        
        public class UploadFile_Response
        {
            /// <summary>
            /// 0 is success, 1 is fail due to auth error, 2 is fail due to lack of parameters, 3 is fail due to invalid file type, 4 is faul due to invalid parameters
            /// </summary>
            public int status_code { get; set; }
            /// <summary>
            /// return an upload_id when success
            /// </summary>
            public int upload_id { get; set; }
            public UploadFile_Response(int status_code, int upload_id = -1)
            {
                this.status_code = status_code;
                this.upload_id = upload_id;
            }
        }
    }
}
