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
    public class RatingController : Controller
    {
        readonly AllDbContext allDbContext;
        public RatingController(AllDbContext allDbContext)
        {
            this.allDbContext = allDbContext;
        }
        /// <summary>
        /// Like, dislike or unlike a file.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [Route("[action]")]
        public async Task<ActionResult<LikeOrUndo_Response>> LikeOrUndo([FromBody]LikeOrUndo_Request request)
        {
            int userid = await allDbContext.Get_User_ID_By_Token(request.auth_token);
            if (userid == -1)
                return new LikeOrUndo_Response(1);
            UploadedFile file = await allDbContext.Get_File_By_FileID(request.targeted_fileID);
            if (file == null)
                return new LikeOrUndo_Response(2);
            Rating rating = await allDbContext.GetRatingByFIDAndUID(userid, request.targeted_fileID);
            if (rating == null)
            {
                rating = new Rating
                {
                    fileID = request.targeted_fileID,
                    userID = userid,
                    user_Rating = request.selected_Option
                };
                if (request.selected_Option == 0)
                    file.DisLikes++;
                else
                    file.Likes++;
                await allDbContext.Ratings.AddAsync(rating);
                await allDbContext.SaveChangesAsync();
                return new LikeOrUndo_Response(0);
            }
            if (rating.user_Rating == request.selected_Option)
            {
                if (request.selected_Option == 0)
                    file.DisLikes--;
                else
                    file.Likes--;
                allDbContext.Ratings.Remove(rating);
                await allDbContext.SaveChangesAsync();
                return new LikeOrUndo_Response(0);
            }
            if (request.selected_Option == 0)
            {
                file.DisLikes++;
                file.Likes--;

            }
            else
            {
                file.DisLikes--;
                file.Likes++;
            }
            rating.user_Rating = request.selected_Option;
            await allDbContext.SaveChangesAsync();
            return new LikeOrUndo_Response(0);
        }
        public class LikeOrUndo_Request
        {
            [Required]
            [StringLength(128, MinimumLength = 128)]
            public string auth_token { get; set; }
            [Required]
            [Range(1, int.MaxValue)]
            public int targeted_fileID { get; set;}
            /// <summary>
            /// <para>0 is dislike, 1 is like</para>
            /// <para>if selected_Option = current rating, then the file will be unliked. (neither like / dislike)</para>
            /// <para>if selected_Option != current rating, then current rating = selected_Option. </para>
            /// <para>Say a user disliked a file and want to like it again, selected_Option should be set to 1.</para>
            /// <para>Say a user disliked a file and want to unlike (neither like / dislike) it, selected_Option should be set to 0.</para>
            /// </summary>
            [Required]
            [Range(0, 1)]
            public int selected_Option { get; set; }
        }
        public class LikeOrUndo_Response
        {
            public LikeOrUndo_Response(int code)
            {
                status_code = code;
            }
            /// <summary>
            /// 0 is success, 1 is auth fail, 2 is invalid target
            /// </summary>
            public int status_code { get; set; }
        }
    }
}