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
    public class ProfileCommentsController : Controller
    {

        readonly AuthTokenContext authTokenContext;
        readonly ProfileCommentsContext profileCommentsContext;
        readonly UserContext userContext;
        public ProfileCommentsController(AuthTokenContext authTokenContext, ProfileCommentsContext profileCommentsContext, UserContext userContext)
        {
            this.authTokenContext = authTokenContext;
            this.profileCommentsContext = profileCommentsContext;
            this.userContext = userContext;
        }
        /// <summary>
        /// Get the profile comments of a user, decreasing ordered by posted time.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [Route("[action]")]
        public async Task<ActionResult<GetProfileComments_Response>> GetProfileComments([FromBody]GetProfileComments_Request request)
        {
            int userid = -1;
            userid = await authTokenContext.Get_User_ID_By_Token(request.auth_token);
            if (userid == -1)
                return new GetProfileComments_Response(1);
            var response = new GetProfileComments_Response(0);
            response.profileComments = await profileCommentsContext.GetComments(request.userid, request.next_20);
            return response;

        }
        /// <summary>
        /// Post a profile comment to a specific user
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [Route("[action]")]
        public async Task<ActionResult<PostProfileComment_Response>> PostProfileComment([FromBody]PostProfileComment_Request request)
        {
            request.text = request.text.Trim();
            if (request.text.Length < 2)
                return new PostProfileComment_Response(3);
            int sender_userid = -1;
            sender_userid = await authTokenContext.Get_User_ID_By_Token(request.auth_token);
            if (sender_userid == -1)
                return new PostProfileComment_Response(1);
            bool target_exist = await userContext.Users.AnyAsync(x => x.ID == request.targeted_userid);
            if (!target_exist)
                return new PostProfileComment_Response(2);
            var comment = new ProfileComment
            {
                posted_dateTime = DateTime.Now,
                sender_UserID = sender_userid,
                receiver_UserID = request.targeted_userid,
                Text = request.text
            };
            await profileCommentsContext.ProfileComments.AddAsync(comment);
            await profileCommentsContext.SaveChangesAsync();
            return new PostProfileComment_Response(0);

        }
        /// <summary>
        /// Delete a profile comment by profilecommentID.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [Route("[action]")]
        public async Task<ActionResult<DeleteProfileComment_Response>> DeleteProfileComment([FromBody]DeleteProfileComment_Request request)
        {
            int userid = -1;
            userid = await authTokenContext.Get_User_ID_By_Token(request.auth_token);
            if (userid == -1)
                return new DeleteProfileComment_Response(1);
            var comment = await profileCommentsContext.GetCommentByID(request.comment_id);
            if (comment == null)
                return new DeleteProfileComment_Response(2);
            if (comment.sender_UserID != userid)
                return new DeleteProfileComment_Response(2);
            profileCommentsContext.ProfileComments.Remove(comment);
            await profileCommentsContext.SaveChangesAsync();
            return new DeleteProfileComment_Response(0);
        }
        public class DeleteProfileComment_Request
        {
            [Required]
            [StringLength(128, MinimumLength = 128)]
            public string auth_token { get; set; }
            [Required]
            public int comment_id { get; set; }
        }
        public class DeleteProfileComment_Response
        {
            public DeleteProfileComment_Response(int code)
            {
                status_code = code;
            }
            /// <summary>
            /// 0 is success, 1 is auth_fail, 2 is not the owner / invalid comment id
            /// </summary>
            public int status_code { get; set; }
        }
        public class PostProfileComment_Request
        {
            [Required]
            [StringLength(128, MinimumLength = 128)]
            public string auth_token { get; set; }
            [Required]
            [Range(0, int.MaxValue)]
            public int targeted_userid { get; set; }
            [Required]
            [StringLength(2048, MinimumLength = 2)]
            public string text { get; set; }
        }
        public class PostProfileComment_Response
        {
            public PostProfileComment_Response(int code)
            {
                status_code = code;
            }
            /// <summary>
            /// 0 is success, 1 is auth fail, 2 is invalid target, 3 is text length is invalid
            /// </summary>
            public int status_code { get; set; }
        }
        public class GetProfileComments_Request
        {
            [Required]
            [StringLength(128, MinimumLength = 128)]
            public string auth_token { get; set; }
            [Required]
            [Range(0, int.MaxValue)]
            public int userid { get; set; }
            /// <summary>
            /// first 20 queries = 0, next 20 queries = 1, next next 20 queries = 2, etc.
            /// </summary>
            [Required]
            [Range(0, int.MaxValue)]
            public int next_20 { get; set; }
        }
        public class GetProfileComments_Response
        {
            public GetProfileComments_Response(int code)
            {
                status_code = code;
            }
            /// <summary>
            /// 0 is success, 1 is auth fail
            /// </summary>
            public int status_code { get; set; }
            public List<ProfileComment> profileComments { get; set; }
        }
    }
}