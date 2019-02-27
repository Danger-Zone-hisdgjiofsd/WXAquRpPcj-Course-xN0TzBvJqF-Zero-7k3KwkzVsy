using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CourseZero.Filters;
using CourseZero.Models;
using CourseZero.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CourseZero.Controllers
{
    [Route("api/[controller]")]
    public class GeneralController : Controller
    {
        readonly UserContext userContext;
        readonly AuthTokenContext authTokenContext;
        public GeneralController(UserContext userContext, AuthTokenContext authTokenContext)
        {
            this.userContext = userContext;
            this.authTokenContext = authTokenContext;
        }
        // GET: /<controller>/
        /// <summary>
        /// Return a list of Course object on success. View class Course in CourseContext.cs for model info. Return "status_code": 1 when fail.
        /// </summary>
        /// <param name="authToken_Request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ServiceFilter(typeof(AuthRequired))]
        public ActionResult GetAllCourses([FromBody]AuthToken_Request authToken_Request)
        {
            return new ContentResult
            {
                Content = CUSIS_Fetch_Service.Get_Course_Json_Str(),
                ContentType = "application/json"
            };
        }
        /// <summary>
        /// Get the username of an user by an userid, if not specifiy the userid, it will return the username which belongs to the auth token user.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<ActionResult<GetUserInfo_Response>> GetUserInfo([FromBody]GetUserInfo_Request request)
        {
            int userID = -1;
            userID = await authTokenContext.Get_User_ID_By_Token(request.auth_token);
            var response = new GetUserInfo_Response();
            if (userID == -1)
            {
                response.status_code = 1;
                return response;
            }
            User user;
            if (request.userid == 0)
            {
                response.status_code = 0;
                user = await userContext.Get_User_By_User_ID(userID);
                response.username = user.username;
            }
            else
            {
                user = await userContext.Get_User_By_User_ID(request.userid);
                if (user == null)
                    response.status_code = 2;
                else
                {
                    response.username = user.username;
                    response.status_code = 0;
                }
            }
            return response;

        }
        public class GetUserInfo_Request
        {
            [Required]
            [StringLength(128, MinimumLength = 128)]
            public string auth_token { get; set; }
            [Range(1, int.MaxValue)]
            public int userid { get; set; }
        }
        public class GetUserInfo_Response
        {
            /// <summary>
            /// 2 is user not found, 1 is auth fail, 0 is success
            /// </summary>
            public int status_code { get; set; }
            public string username { get; set; }
        }
    }
}
