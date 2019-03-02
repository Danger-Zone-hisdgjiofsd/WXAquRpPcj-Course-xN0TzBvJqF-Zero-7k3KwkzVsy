﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CourseZero.Models;
using CourseZero.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseZero.Controllers
{
    [Route("api/[controller]")]
    public class WatchLaterController : Controller
    {
        readonly AuthTokenContext authTokenContext;
        readonly WatchLaterContext waterLaterContext;
        public WatchLaterController(AuthTokenContext authTokenContext, WatchLaterContext waterLaterContext)
        {
            this.authTokenContext = authTokenContext;
            this.waterLaterContext = waterLaterContext;
        }
        /// <summary>
        /// Add to a file to watch later list if it is not added currently. Unubscribe to a course if it is added currently.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [Route("[action]")]
        public async Task<ActionResult<AddWatchLaterOrUndo_Response>> AddWatchLaterOrUndo([FromBody]AddWatchLaterOrUndo_Request request)
        {
            int userid = -1;
            userid = await authTokenContext.Get_User_ID_By_Token(request.auth_token);
            if (userid == -1)
                return new AddWatchLaterOrUndo_Response(1);
            var result = await waterLaterContext.watchLaters.FirstOrDefaultAsync(x => x.UserID == userid && x.FileID == request.fileid);
            if (result == null)
            {
                result = new WatchLater();
                result.FileID = request.fileid;
                result.UserID = userid;
                await waterLaterContext.watchLaters.AddAsync(result);
                await waterLaterContext.SaveChangesAsync();
                return new AddWatchLaterOrUndo_Response(2);
            }
            waterLaterContext.watchLaters.Remove(result);
            await waterLaterContext.SaveChangesAsync();
            return new AddWatchLaterOrUndo_Response(3);
        }
        /// <summary>
        /// Return a list of watch later file id. Return unauthorised if auth fail.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [Route("[action]")]
        public async Task<ActionResult<GetUserWatchLater_Response>> GetUserWatchLater([FromBody]GetUserWatchLater_Request request)
        {
            int userid = -1;
            userid = await authTokenContext.Get_User_ID_By_Token(request.auth_token);
            if (userid == -1)
                return new UnauthorizedResult();
            var results = await waterLaterContext.GetAllWatchLater(userid, request.next_20);
            var response = new GetUserWatchLater_Response();
            response.FileID = results;
            return response;
        }
        public class AddWatchLaterOrUndo_Request
        {
            [Required]
            [StringLength(128, MinimumLength = 128)]
            public string auth_token { get; set; }
            [Required]
            [Range(1, int.MaxValue)]
            public int fileid { get; set; }
        }
        public class AddWatchLaterOrUndo_Response
        {
            public AddWatchLaterOrUndo_Response(int code)
            {
                status_code = code;
            }
            /// <summary>
            /// 1 is auth fail, 2 is added, 3 is removed
            /// </summary>
            public int status_code { get; set; }
        }
        public class GetUserWatchLater_Request
        {
            [Required]
            [StringLength(128, MinimumLength = 128)]
            public string auth_token { get; set; }
            /// <summary>
            /// first 20 queries = 0, next 20 queries = 1, next next 20 queries = 2, etc.
            /// </summary>
            [Required]
            [Range(0, int.MaxValue)]
            public int next_20 { get; set; }
        }
        public class GetUserWatchLater_Response
        {
            public List<int> FileID { get; set;}
        }
    }
}