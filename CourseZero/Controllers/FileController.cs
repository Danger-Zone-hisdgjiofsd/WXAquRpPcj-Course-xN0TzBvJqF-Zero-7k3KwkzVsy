using CourseZero.Filters;
using CourseZero.Models;
using CourseZero.Services;
using CourseZero.Tools;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseZero.Controllers
{
    [Route("api/[controller]")]
    public class FileController : Controller
    {
        readonly UploadedFileContext uploadedFileContext;
        readonly AuthTokenContext authTokenContext;
        public FileController(UploadedFileContext uploadedFileContext, AuthTokenContext authTokenContext)
        {
            this.uploadedFileContext = uploadedFileContext;
            this.authTokenContext = authTokenContext;
        }
        /// <summary>
        /// Edit an uploaded file info
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<EditFileInfo_Response> EditFileInfo([FromBody]EditFileInfo_Request request)
        {
            request.file_Name = request.file_Name.Trim();
            request.file_Description = request.file_Description.Trim();
            int userid = -1;
            userid = await authTokenContext.Get_User_ID_By_Token(request.auth_token);
            if (userid == -1)
                return new EditFileInfo_Response(1);
            if (CUSIS_Fetch_Service.CourseID_range.lower > request.related_courseID || request.related_courseID > CUSIS_Fetch_Service.CourseID_range.upper || request.file_Name.Length == 0)
                return new EditFileInfo_Response(3);
            var file = await uploadedFileContext.Get_File_By_FileID(request.file_ID);
            if (file == null)
                return new EditFileInfo_Response(2);
            if (file.Uploader_UserID != userid)
                return new EditFileInfo_Response(2);
            if (File_Process_Tool.File_Non_Scannable(file.File_Typename) && request.file_Description.Length < 10)
                return new EditFileInfo_Response(3);
            file.File_Name = request.file_Name;
            file.File_Description = request.file_Description;
            file.Related_courseID = request.related_courseID;
            var course = CUSIS_Fetch_Service.GetCourse_By_CourseID(request.related_courseID);
            StringBuilder stringBuider = new StringBuilder();
            stringBuider.Append(course.Prefix + " ");
            stringBuider.Append(course.Course_Code + " ");
            stringBuider.Append(course.Course_Title + " ");
            stringBuider.Append(course.Subject_Name + " ");
            stringBuider.Append(request.file_Description + " ");
            stringBuider.Append(request.file_Name + " ");
            file.Words_for_Search = stringBuider.ToString();
            await uploadedFileContext.SaveChangesAsync();
            return new EditFileInfo_Response(0);
        }
        /// <summary>
        /// Delete an uploaded file
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<DeleteFile_Response> DeleteFile([FromBody]DeleteFile_Request request)
        {
            int userid = -1;
            userid = await authTokenContext.Get_User_ID_By_Token(request.auth_token);
            if (userid == -1)
                return new DeleteFile_Response(1);
            var file = await uploadedFileContext.Get_File_By_FileID(request.file_ID);
            if (file == null)
                return new DeleteFile_Response(2);
            if (file.Uploader_UserID != userid)
                return new DeleteFile_Response(2);
            if (!file.Stored_Internally)
                System.IO.File.Delete(AppDomain.CurrentDomain.BaseDirectory + "/Uploads/" + file.ID + file.File_Typename);
            System.IO.File.Delete(AppDomain.CurrentDomain.BaseDirectory + "/UploadsThumbnail/" + file.ID + ".png");
            uploadedFileContext.UploadedFiles.Remove(file);
            await uploadedFileContext.SaveChangesAsync();
            return new DeleteFile_Response(0);
        }
        [HttpPost]
        [Route("[action]")]
        [Consumes("application/json")]
        [ServiceFilter(typeof(AuthRequired))]
        public async Task<IActionResult> GetFileByFileid([FromBody]GetFileByFileid_Reuqest request)
        {
            UploadedFile requested_file = await uploadedFileContext.Get_File_By_FileID(request.file_ID);
            if (requested_file == null)
                return new NotFoundResult();
            if (requested_file.Stored_Internally)
                return File(requested_file.Binary, "application/octet-stream");
            else if (System.IO.File.Exists((AppDomain.CurrentDomain.BaseDirectory + "Uploads/" + request.file_ID + requested_file.File_Typename)))
                return PhysicalFile(AppDomain.CurrentDomain.BaseDirectory + "Uploads/" + request.file_ID + requested_file.File_Typename, "application/octet-stream");
            return new NotFoundResult();
        }
        /// <summary>
        /// Return file Thumbnail (.png) generated by the server
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        [Consumes("application/json")]
        [ServiceFilter(typeof(AuthRequired))]
        public IActionResult GetThumbnailByFileid([FromBody]GetThumbnailByFileid_Request request)
        {
            if (System.IO.File.Exists((AppDomain.CurrentDomain.BaseDirectory + "UploadsThumbnail/" + request.file_ID + ".png")))
                return PhysicalFile(AppDomain.CurrentDomain.BaseDirectory + "UploadsThumbnail/" + request.file_ID + ".png", "application/octet-stream");
            return new NotFoundResult();
        }
        public class GetFileByFileid_Reuqest
        {
            [Required]
            [StringLength(128, MinimumLength = 128)]
            public string auth_token { get; set; }
            [Required]
            public int file_ID { get; set; }
        }
        public class GetThumbnailByFileid_Request
        {
            [Required]
            [StringLength(128, MinimumLength = 128)]
            public string auth_token { get; set; }
            [Required]
            public int file_ID { get; set; }
        }
        public class EditFileInfo_Request
        {
            [Required]
            [StringLength(128, MinimumLength = 128)]
            public string auth_token { get; set; }
            [Required]
            public int file_ID { get; set; }
            /// <summary>
            /// Length has to be > 0
            /// </summary>
            [Required]
            public string file_Name { get; set; }
            /// <summary>
            /// For non scannable file, length has to be >= 10, otherwise >= 0.
            /// </summary>
            [Required]
            public string file_Description { get; set; }
            [Required]
            public int related_courseID { get; set; }
        }
        public class EditFileInfo_Response
        {
            public EditFileInfo_Response(int code)
            {
                status_code = code;
            }
            /// <summary>
            /// 0 is success, 1 is auth fail, 2 is file does not exit / not the owner, 3 is inputs are invalid
            /// </summary>
            public int status_code { get; set; }
        }
        public class DeleteFile_Request
        {
            [Required]
            [StringLength(128, MinimumLength = 128)]
            public string auth_token { get; set; }
            [Required]
            public int file_ID { get; set; }
        }
        public class DeleteFile_Response
        {
            public DeleteFile_Response(int code)
            {
                status_code = code;
            }
            /// <summary>
            /// 0 is success, 1 is auth fail, 2 is file does not exit / not the owner
            /// </summary>
            public int status_code { get; set; }
        }
    }
}
