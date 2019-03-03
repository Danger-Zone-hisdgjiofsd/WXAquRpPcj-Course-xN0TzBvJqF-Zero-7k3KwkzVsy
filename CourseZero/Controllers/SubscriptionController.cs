using System;
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
    public class SubscriptionController : Controller
    {
        readonly AllDbContext allDbContext;
        public SubscriptionController(AllDbContext allDbContext)
        {
            this.allDbContext = allDbContext;
        }
        /// <summary>
        /// Subscribe to a course if it is not subscribed currently. Unubscribe to a course if it is subscribed currently. Max limit per user is 200.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [Route("[action]")]
        public async Task<ActionResult<SubscribeOrUndo_Response>> SubscribeOrUndo([FromBody]SubscribeOrUndo_Request request)
        {
            if (CUSIS_Fetch_Service.CourseID_range.lower > request.courseid || CUSIS_Fetch_Service.CourseID_range.upper < request.courseid)
                return new SubscribeOrUndo_Response(4);
            int userid = -1;
            userid = await allDbContext.Get_User_ID_By_Token(request.auth_token);
            if (userid == -1)
                return new SubscribeOrUndo_Response(1);
            int count = await allDbContext.Subscriptions.CountAsync(x => x.UserID == userid);
            if (count == 200)
                return new SubscribeOrUndo_Response(5);
            var result = await allDbContext.Subscriptions.FirstOrDefaultAsync(x => x.UserID == userid && x.CourseID == request.courseid);
            if (result == null)
            {
                result = new Subscription();
                result.CourseID = request.courseid;
                result.UserID = userid;
                await allDbContext.Subscriptions.AddAsync(result);
                await allDbContext.SaveChangesAsync();
                return new SubscribeOrUndo_Response(2);
            }
            allDbContext.Subscriptions.Remove(result);
            await allDbContext.SaveChangesAsync();
            return new SubscribeOrUndo_Response(3);
        }
        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [Route("[action]")]
        public async Task<ActionResult<GetUserAllSubscription_Response>> GetUserAllSubscription([FromBody]GetUserAllSubscription_Request request)
        {
            int userid = -1;
            userid = await allDbContext.Get_User_ID_By_Token(request.auth_token);
            if (userid == -1)
                return new UnauthorizedResult();
            var results = await allDbContext.GetAllSubscriptions(userid);
            var response = new GetUserAllSubscription_Response();
            response.CourseIDs = results;
            return response;
        }
        public class SubscribeOrUndo_Request
        {
            [Required]
            [StringLength(128, MinimumLength = 128)]
            public string auth_token { get; set; }
            [Required]
            [Range(1, int.MaxValue)]
            public int courseid { get; set; }
        }
        public class SubscribeOrUndo_Response
        {
            public SubscribeOrUndo_Response(int code)
            {
                status_code = code;
            }
            /// <summary>
            /// 1 is auth fail, 2 is subscribed, 3 is unsubscribed, 4 invalid courseid, 5 is fail due to hitting the limit 200
            /// </summary>
            public int status_code { get; set; }
        }
        public class GetUserAllSubscription_Request
        {
            [Required]
            [StringLength(128, MinimumLength = 128)]
            public string auth_token { get; set; }
        }
        public class GetUserAllSubscription_Response
        {
            public List<int> CourseIDs { get; set;}
        }
    }
}