using System;
using System.Collections.Generic;
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
        [HttpPost]
        [Route("[action]")]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<ActionResult<GetUserInfo_Response>> GetUserInfo([FromBody]AuthToken_Request authToken_Request)
        {
            int userID = -1;
            userID = await authTokenContext.Get_User_ID_By_Token(authToken_Request.auth_token);
            var response = new GetUserInfo_Response();
            if (userID == -1)
            {
                response.status_code = 1;
                return response;
            }
            response.status_code = 0;
            var User = await userContext.Get_User_By_User_ID(userID);
            response.username = User.username;
            return response;

        }
        public class GetUserInfo_Response
        {
            /// <summary>
            /// 1 is fail, 0 is success
            /// </summary>
            public int status_code { get; set; }
            public string username { get; set; }
        }
    }
}
