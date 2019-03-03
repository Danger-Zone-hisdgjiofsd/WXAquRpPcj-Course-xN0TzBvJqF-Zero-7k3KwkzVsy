using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CourseZero.Filters;
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

        readonly AllDbContext allDbContext;
        public UploadController(AllDbContext allDbContext)
        {
            this.allDbContext = allDbContext;
        }
        /// <summary>
        /// Get the file process status by upload hist (file) id.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [Route("[action]")]
        public async Task<ActionResult<GetFileProcessStatus_Response>> GetFileProcessStatus([FromBody]GetFileProcessStatus_Request request)
        {
            var response = new GetFileProcessStatus_Response();
            int userid = -1;
            userid = await allDbContext.Get_User_ID_By_Token(request.auth_token);
            if (userid == -1)
            {
                response.status_code = 1;
                return response;
            }
            if (File_Process_Service.Current_ProcessObj != null)
            {
                if (File_Process_Service.Current_ProcessObj.ID == request.upload_hist_ID)
                {
                    if (userid != File_Process_Service.Current_ProcessObj.Uploader_UserID)
                    {
                        response.status_code = 3;
                        return response;
                    }
                    response.processed = false;
                    response.status_code = 0;
                    response.queue_pos = 0;
                    return response;
                }
            }
            UploadHist upload_history_obj;
            if (File_Process_Service.Queue_Position.ContainsKey(request.upload_hist_ID))
            {
                upload_history_obj = File_Process_Service.Queue_Position[request.upload_hist_ID].uploadhist;
                if (userid != upload_history_obj.Uploader_UserID)
                {
                    response.status_code = 3;
                    return response;
                }
                response.processed = false;
                response.status_code = 0;
                response.queue_total = File_Process_Service.Queue_Position.Count;
                response.queue_pos = File_Process_Service.Queue_Position[request.upload_hist_ID].queue;
                return response;
            }
            upload_history_obj = await allDbContext.Get_UploadHist_By_Hist_ID(request.upload_hist_ID);
            if (upload_history_obj == null)
            {
                response.status_code = 2;
                return response;
            }
            if (userid != upload_history_obj.Uploader_UserID)
            {
                response.status_code = 3;
                return response;
            }
            if (!upload_history_obj.Processed)
            {
                response.status_code = 4;
                return response;
            }
            response.processed = true;
            response.uploadHist = upload_history_obj;
            return response;
        }


        /// <summary>
        /// Keys are (first one MUST be auth_token):
        /// auth_token,
        /// file_name [len not longer than 256],
        /// file_description [len not longer than 10240],
        /// related_courseid,
        /// file,
        /// [file has to be less than 100MB]
        /// 
        /// (If the file name is with length 0, server will use the uploaded file name instead.)
        /// (non scannable(".wav", ".mp3", ".3gp", ".mp4", ".avi", ".mkv") file must has file_description with length larger than or equal to 10)
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
                    if (File_Process_Tool.File_Non_Scannable(type) && file_description.Length < 10)
                        return new UploadFile_Response(5);
                    if (file_name.Length == 0)
                    {
                        file_name = fileSection.FileName;
                        file_name = file_name.Substring(0, dot_index);
                    }
                    uploadHist.Uploader_UserID = userID;
                    uploadHist.Upload_Time = DateTime.Now;
                    uploadHist.Processed = false;
                    uploadHist.File_Name = file_name;
                    uploadHist.File_typename = type;
                    uploadHist.Related_courseID = courseID;
                    uploadHist.File_Description = file_description;
                    await allDbContext.UploadHistories.AddAsync(uploadHist);
                    await allDbContext.SaveChangesAsync();
                    using (var stream = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "/UploadsQueue/" + uploadHist.ID + type, FileMode.CreateNew))
                        await fileSection.FileStream.CopyToAsync(stream);
                }
                else if (contentDispo.IsFormDisposition())
                {
                    var formSection = section.AsFormDataSection();
                    var value = await formSection.GetValueAsync();
                    if (formSection.Name == "auth_token" && !auth_found && value.Length == 128)
                    {
                        userID = await allDbContext.Get_User_ID_By_Token(value);
                        if (userID == -1)
                            return new UploadFile_Response(1);
                        auth_found = true;
                    }
                    if (formSection.Name == "file_name" && value.Length <= 256)
                        file_name = value.Trim();
                    if (formSection.Name == "file_description" && value.Length <= 10240)
                        file_description = value.Trim();
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
                File_Process_Service.Enqueue(uploadHist);
                return new UploadFile_Response(0, uploadHist.ID);
            }
            return new UploadFile_Response(2);
        }
        
        public class UploadFile_Response
        {
            /// <summary>
            /// 0 is success, 1 is fail due to auth error, 2 is fail due to lack of parameters, 3 is fail due to invalid file type, 4 is faul due to invalid parameters, 5 file is non scannable(".wav", ".mp3", ".3gp", ".mp4", ".avi", ".mkv") but the user does not provide File_Description with length >= 10
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
        public class GetFileProcessStatus_Request
        {
            [Required]
            [StringLength(128, MinimumLength = 128)]
            public string auth_token { get; set; }
            [Required]
            [Range(1, int.MaxValue)]
            public int upload_hist_ID { get; set; }
        }
        public class GetFileProcessStatus_Response
        {
            /// <summary>
            /// 0 is success, 1 is auth fail, 2 is invalid upload_hist_ID, 3 is not the owner of upload_hist_ID, 4 is not processed but not in the process queue (should not exist in real case if no one disturb the server)
            /// </summary>
            public int status_code { get; set; }
            /// <summary>
            /// true if the file has been processed.
            /// </summary>
            public bool processed { get; set; }
            /// <summary>
            /// If the file is processed, Object UploadHist will be returned.
            /// </summary>
            public UploadHist uploadHist { get; set; }
            /// <summary>
            /// If file is not processed, this will be the queue position. 0 means currently processing. 1, 2, 3, 4 ... mean current queue position (maybe useful in designing the progress bar)
            /// </summary>
            public int queue_pos { get; set; }
            /// <summary>
            /// If file is not processed, this will be the total number of file waiting in the queue. If queue_pos is 0, this will be 0.  (maybe useful in designing the progress bar)
            /// </summary>
            public int queue_total { get; set; }

        }
    }
}
