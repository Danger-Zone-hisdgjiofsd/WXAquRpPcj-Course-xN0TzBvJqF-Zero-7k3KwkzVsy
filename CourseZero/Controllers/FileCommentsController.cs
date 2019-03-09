using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CourseZero.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseZero.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileCommentsController : Controller
    {

        readonly AllDbContext allDbContext;
        public FileCommentsController(AllDbContext allDbContext)
        {
            this.allDbContext = allDbContext;
        }
        /// <summary>
        /// Get the file comments of a specific file, decreasing ordered by posted time.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [Route("[action]")]
        public async Task<ActionResult<GetFileComments_Response>> GetFileComments([FromBody]GetFileComments_Request request)
        {
            if (await allDbContext.Get_User_ID_By_Token(request.auth_token) == -1)
                return new GetFileComments_Response(1);
            var response = new GetFileComments_Response(0);
            response.FileComments = await allDbContext.FileComments.Where(x => x.file_ID == request.fileid).OrderByDescending(x => x.ID).Skip(request.next_20 * 20).Take(20).Join(
                allDbContext.Users, x => x.sender_UserID, y => y.ID, (x, y)=> new FileComment_ShownToUser
                {
                    ID = x.ID,
                    Text = x.Text,
                    posted_dateTime = x.posted_dateTime,
                    sender_UserID = x.sender_UserID,
                    sender_Username = y.username
                }).ToListAsync();
            return response;

        }
        /// <summary>
        /// Post a file comment
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [Route("[action]")]
        public async Task<ActionResult<PostFileComment_Response>> PostFileComment([FromBody]PostFileComment_Request request)
        {
            request.text = request.text.Trim();
            if (request.text.Length < 2)
                return new PostFileComment_Response(3);
            int sender_userid = -1;
            sender_userid = await allDbContext.Get_User_ID_By_Token(request.auth_token);
            if (sender_userid == -1)
                return new PostFileComment_Response(1);
            bool target_exist = await allDbContext.UploadedFiles.AnyAsync(x => x.ID == request.targeted_fileid);
            if (!target_exist)
                return new PostFileComment_Response(2);
            var comment = new FileComment
            {
                posted_dateTime = DateTime.Now,
                sender_UserID = sender_userid,
                file_ID = request.targeted_fileid,
                Text = request.text
            };
            await allDbContext.FileComments.AddAsync(comment);
            await allDbContext.SaveChangesAsync();
            return new PostFileComment_Response(0);

        }
        /// <summary>
        /// Delete a comment by filecommentID.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [Route("[action]")]
        public async Task<ActionResult<DeleteFileComment_Response>> DeleteFileComment([FromBody]DeleteFileComment_Request request)
        {
            int userid = -1;
            userid = await allDbContext.Get_User_ID_By_Token(request.auth_token);
            if (userid == -1)
                return new DeleteFileComment_Response(1);
            var comment = await allDbContext.GetFileCommentByID(request.comment_id);
            if (comment == null)
                return new DeleteFileComment_Response(2);
            if (comment.sender_UserID != userid)
                return new DeleteFileComment_Response(2);
            allDbContext.FileComments.Remove(comment);
            await allDbContext.SaveChangesAsync();
            return new DeleteFileComment_Response(0);
        }
        public class DeleteFileComment_Request
        {
            [Required]
            [StringLength(128, MinimumLength = 128)]
            public string auth_token { get; set; }
            [Required]
            public int comment_id { get; set; }
        }
        public class DeleteFileComment_Response
        {
            public DeleteFileComment_Response(int code)
            {
                status_code = code;
            }
            /// <summary>
            /// 0 is success, 1 is auth_fail, 2 is not the owner / invalid comment id
            /// </summary>
            public int status_code { get; set; }
        }
        public class PostFileComment_Request
        {
            [Required]
            [StringLength(128, MinimumLength = 128)]
            public string auth_token { get; set; }
            [Required]
            [Range(0, int.MaxValue)]
            public int targeted_fileid { get; set; }
            [Required]
            [StringLength(2048, MinimumLength = 2)]
            public string text { get; set; }
        }
        public class PostFileComment_Response
        {
            public PostFileComment_Response(int code)
            {
                status_code = code;
            }
            /// <summary>
            /// 0 is success, 1 is auth fail, 2 is invalid target, 3 is text length is invalid
            /// </summary>
            public int status_code { get; set; }
        }
        public class GetFileComments_Request
        {
            [Required]
            [StringLength(128, MinimumLength = 128)]
            public string auth_token { get; set; }
            [Required]
            [Range(0, int.MaxValue)]
            public int fileid { get; set; }
            /// <summary>
            /// first 20 queries = 0, next 20 queries = 1, next next 20 queries = 2, etc.
            /// </summary>
            [Required]
            [Range(0, int.MaxValue)]
            public int next_20 { get; set; }
        }
        public class GetFileComments_Response
        {
            public GetFileComments_Response(int code)
            {
                status_code = code;
            }
            /// <summary>
            /// 0 is success, 1 is auth fail
            /// </summary>
            public int status_code { get; set; }
            public List<FileComment_ShownToUser> FileComments { get; set; }
        }
    }
}