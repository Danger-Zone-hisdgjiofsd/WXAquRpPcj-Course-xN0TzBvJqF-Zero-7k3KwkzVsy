using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CourseZero.Models;
using Microsoft.AspNetCore.Mvc;

namespace CourseZero.Controllers
{
    [Route("api/[controller]")]
    public class ReportController : Controller
    {
        readonly AllDbContext allDbContext;
        public ReportController(AllDbContext allDbContext)
        {
            this.allDbContext = allDbContext;
        }
        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [Route("[action]")]
        public async Task<ActionResult<SendReport_Response>> SendReport([FromBody] SendReport_Request request)
        {
            var response = new SendReport_Response();
            int userid = await allDbContext.Get_User_ID_By_Token(request.auth_token);
            if (userid == -1)
            {
                response.status_code = 1;
                return response;
            }
            var report = new Report
            {
                RelatedID = request.related_id,
                Text = request.text,
                Report_Type = request.report_type,
                UserID = userid,
                ReportTime = DateTime.Now,
                Resovled = false
            };
            await allDbContext.Reports.AddAsync(report);
            await allDbContext.SaveChangesAsync();
            return response;
        }
        public class SendReport_Request
        {
            [Required]
            [StringLength(128, MinimumLength = 128)]
            public string auth_token { get; set; }
            /// <summary>
            /// 0 is file, 1 is user, 2 is file comment, 3 is profile comment, 4 is general
            /// </summary>
            [Required]
            [Range(0, 4)]
            public int report_type { get; set; }
            /// <summary>
            /// provide the id of the file/ comment/ user. No need for general.
            /// </summary>
            [Range(1, int.MaxValue)]
            public int related_id { get; set; }
            [Required]
            [MaxLength(10240)]
            public string text { get; set; }
        }
        public class SendReport_Response
        {
            /// <summary>
            /// 0 is success, 1 is auth fail
            /// </summary>
            public int status_code { get; set; }
        }
    }
}