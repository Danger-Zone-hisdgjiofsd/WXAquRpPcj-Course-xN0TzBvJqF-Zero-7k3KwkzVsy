using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using CourseZero.Filters;
using CourseZero.Models;
using CourseZero.Tools;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Z.Linq;

namespace CourseZero.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : Controller
    {
        readonly UploadedFileContext uploadedFileContext;
        public SearchController(UploadedFileContext uploadedFileContext)
        {
            this.uploadedFileContext = uploadedFileContext;
        }
        /// <summary>
        /// A search method that will return a list of result according to ranking. Top one is the most relevant.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [Route("[action]")]
        [ServiceFilter(typeof(AuthRequired))]
        public async Task<ActionResult<Search_Response>> SearchByQuery([FromBody]Search_Request request)
        {
            var response = new Search_Response();
            var response_result = new List<File_Shown_to_User>();
            request.search_query = request.search_query.Trim();
            if (request.search_query.Length < 5)
            {
                response.status_code = 2;
                return response;
            }
            string where_condition = "WHERE ";
            SqlParameter courseid_parameter = new SqlParameter("CourseIDValue", "");
            SqlParameter filetype_parameter = new SqlParameter("FileTypeValue", "");
            SqlParameter shouldbefore_parameter = new SqlParameter("ShouldBeforeValue", "");
            SqlParameter shouldafter_parameter = new SqlParameter("ShouldAfterValue", "");
            int where_count = 0;
            if (request.specific_course != null)
            {
                if (request.specific_course.Count >= 10 || request.specific_course.Count == 0)
                {
                    response.status_code = 3;
                    return response;
                }
                where_count++;
                if (where_count > 1)
                    where_condition += " AND ";
                where_condition += "[Related_courseID] IN (select value from openjson(@CourseIDValue))";
                courseid_parameter.Value = JsonConvert.SerializeObject(request.specific_course);
            }
            if (request.specific_filetype != null)
            {
                if (request.specific_filetype.Count == 0)
                {
                    response.status_code = 4;
                    return response;
                }
                foreach (var type in request.specific_filetype)
                {
                    if (!File_Process_Tool.File_Allowed(type))
                    {
                        response.status_code = 4;
                        return response;
                    }
                    where_count++;
                    if (where_count > 1)
                        where_condition += " AND ";
                    where_condition += "[File_Typename] IN (select value from openjson(@FileTypeValue))";
                    filetype_parameter.Value = JsonConvert.SerializeObject(request.specific_filetype);
                }
            }
            if (request.should_before != default(DateTime))
            {
                if (DateTime.Compare(request.should_before, (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue) < 0 || DateTime.Compare(request.should_before, (DateTime)System.Data.SqlTypes.SqlDateTime.MaxValue) > 0)
                {
                    response.status_code = 5;
                    return response;
                }
                where_count++;
                if (where_count > 1)
                    where_condition += " AND ";
                where_condition += "[Upload_Time] <= @ShouldBeforeValue";
                shouldbefore_parameter.Value = request.should_before;
            }
            if (request.should_after != default(DateTime))
            {
                if (DateTime.Compare(request.should_after, (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue) < 0 || DateTime.Compare(request.should_after, (DateTime)System.Data.SqlTypes.SqlDateTime.MaxValue) > 0)
                {
                    response.status_code = 5;
                    return response;
                }
                where_count++;
                if (where_count > 1)
                    where_condition += " AND ";
                where_condition += "[Upload_Time] >= @ShouldAfterValue";
                shouldafter_parameter.Value = request.should_after;
            }
            request.next_20 *= 20;
            var database_result = uploadedFileContext.UploadedFiles.FromSql("SELECT ftt.[RANK], v.* FROM FREETEXTTABLE ([CourseZero].[dbo].[UploadedFiles], ([Binary], [Words_for_Search]), @search_query) ftt INNER JOIN [CourseZero].[dbo].[UploadedFiles] v ON v.ID = ftt.[KEY] " + ((where_count>0)?where_condition:"") +" ORDER BY ftt.[RANK] DESC OFFSET @next_20 ROWS FETCH NEXT 20 ROWS ONLY;",
                new SqlParameter("search_query", request.search_query), new SqlParameter("next_20", request.next_20), courseid_parameter, filetype_parameter, shouldbefore_parameter, shouldafter_parameter);
            foreach (var result in database_result)
            {
                File_Shown_to_User file_Shown_To_User = new File_Shown_to_User
                {
                    DisLikes = result.DisLikes,
                    Likes = result.Likes,
                    File_Description = result.File_Description,
                    File_ID = result.ID,
                    File_Name = result.File_Name,
                    File_Typename = result.File_Typename,
                    Related_courseID = result.Related_courseID,
                    Uploader_UserID = result.Uploader_UserID,
                    Upload_Time = result.Upload_Time
                };
                response_result.Add(file_Shown_To_User);
            }
            response.result = response_result;
            return response;
            
        }
        public class Search_Response
        {
            public int status_code { get; set; }
            public List<File_Shown_to_User> result { get; set; }
        }
        public class Search_Request
        {
            [Required]
            [StringLength(128, MinimumLength = 128)]
            public string auth_token { get; set; }
            /// <summary>
            /// <para>text which the user typed in the search box</para>
            /// <para>the query has to contain at least 5 letters, otherwise status_code = 2</para>
            /// </summary>
            [Required]
            [StringLength(256, MinimumLength = 5)]
            public string search_query { get; set; }
            /// <summary>
            /// <para>List of targeted courseID</para>
            /// <para>Maximum specific course is 10, otherwise status_code = 3</para>
            /// </summary>
            public List<int> specific_course { get; set; }
            /// <summary>
            /// <para>uploaded time should before or equal this value</para>
            /// <para> status_code = 5 if invalid (not between 1/1/1753 12:00:00 - 12/31/9999 11:59:59) </para>
            /// </summary>
            public DateTime should_before { get; set; }
            /// <summary>
            /// <para>uploaded time should after or equal this value</para>
            /// <para> status_code = 5 if invalid (not between 1/1/1753 12:00:00 - 12/31/9999 11:59:59) </para>
            /// </summary>
            public DateTime should_after { get; set; }
            /// <summary>
            /// <para>List of filetype</para>
            /// <para>return status_code = 4 if invalid </para>
            /// </summary>
            public List<string> specific_filetype { get; set; }
            /// <summary>
            /// first 20 queries = 0, next 20 queries = 1, next next 20 queries = 2, etc.
            /// </summary>
            [Required]
            [Range(0, int.MaxValue)]
            public int next_20 { get; set; }
        }
    }
}