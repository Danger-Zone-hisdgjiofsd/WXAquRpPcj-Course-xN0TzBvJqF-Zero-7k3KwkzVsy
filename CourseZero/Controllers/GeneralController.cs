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
        public GeneralController()
        {
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
    }
}
