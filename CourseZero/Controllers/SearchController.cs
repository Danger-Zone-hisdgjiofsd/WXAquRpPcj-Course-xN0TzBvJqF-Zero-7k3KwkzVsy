using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseZero.Filters;
using CourseZero.Models;
using CourseZero.Services;
using CourseZero.Tools;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

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
        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [Route("[action]")]
        [ServiceFilter(typeof(AuthRequired))]
        public async Task<ActionResult<Search_Response>> SearchByUserID([FromBody]SearchByUserID_Request request)
        {
            var response = new Search_Response();
            var response_result = new List<File_Shown_to_User>();
            StringBuilder where_condition = new StringBuilder();
            where_condition.Append("WHERE ");
            SqlParameter userid_parameter = new SqlParameter("UserIDValue", "");
            SqlParameter filetype_parameter = new SqlParameter("FileTypeValue", "");
            SqlParameter shouldbefore_parameter = new SqlParameter("ShouldBeforeValue", "");
            SqlParameter shouldafter_parameter = new SqlParameter("ShouldAfterValue", "");
            where_condition.Append("[Uploader_UserID] = @UserIDValue");
            userid_parameter.Value = request.userid;
            Console.WriteLine(userid_parameter.Value);
            if (request.specific_filetype != null)
            {
                if (request.specific_filetype.Count == 0)
                    return new Search_Response(4);
                request.specific_filetype = request.specific_filetype.Distinct().ToList();
                foreach (var type in request.specific_filetype)
                {
                    if (!File_Process_Tool.File_Allowed(type))
                        return new Search_Response(4);
                }
                where_condition.Append(" AND ");
                where_condition.Append("[File_Typename] IN (select value from openjson(@FileTypeValue))");
                filetype_parameter.Value = JsonConvert.SerializeObject(request.specific_filetype);
            }
            if (request.should_before != default(DateTime))
            {
                if (Invalid_Datetime(request.should_before))
                    return new Search_Response(5);
                where_condition.Append("[Upload_Time] <= @ShouldBeforeValue");
                shouldbefore_parameter.Value = request.should_before;
            }
            if (request.should_after != default(DateTime))
            {
                if (Invalid_Datetime(request.should_after))
                    return new Search_Response(5);
                where_condition.Append("[Upload_Time] >= @ShouldAfterValue");
                shouldafter_parameter.Value = request.should_after;
            }
            string orderby_str = "";
            if (request.order_by == 0)
            {
                if (request.order == 0)
                    orderby_str = " ORDER BY [Upload_Time] DESC";
                else
                    orderby_str = " ORDER BY [Upload_Time] ASC";
            }
            else
            {
                if (request.order == 0)
                    orderby_str = " ORDER BY [Likes] DESC";
                else
                    orderby_str = " ORDER BY [Likes] ASC";
            }
            request.next_20 *= 20;
            IQueryable<UploadedFile> database_result = uploadedFileContext.UploadedFiles.FromSql("SELECT * FROM [CourseZero].[dbo].[UploadedFiles] " + where_condition.ToString() + orderby_str + " OFFSET @next_20 ROWS FETCH NEXT 20 ROWS ONLY;",
                 new SqlParameter("next_20", request.next_20), userid_parameter, filetype_parameter, shouldbefore_parameter, shouldafter_parameter);
            foreach (var result in database_result)
                Add_Result_To_Response(result, response_result);
            response.result = response_result;
            return response;
        }
        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [Route("[action]")]
        [ServiceFilter(typeof(AuthRequired))]
        public async Task<ActionResult<Search_Response>> SearchByCourseID([FromBody]SearchByCourseID_Request request)
        {
            var response = new Search_Response();
            var response_result = new List<File_Shown_to_User>();
            
            if (request.specific_course.Count >= 10 || request.specific_course.Count == 0)
                return new Search_Response(3);
            if (Invalid_CourseID(request.specific_course))
                return new Search_Response(3);
            StringBuilder where_condition = new StringBuilder();
            where_condition.Append("WHERE ");
            SqlParameter courseid_parameter = new SqlParameter("CourseIDValue", "");
            SqlParameter filetype_parameter = new SqlParameter("FileTypeValue", "");
            SqlParameter shouldbefore_parameter = new SqlParameter("ShouldBeforeValue", "");
            SqlParameter shouldafter_parameter = new SqlParameter("ShouldAfterValue", "");
            where_condition.Append("[Related_courseID] IN (select value from openjson(@CourseIDValue))");
            courseid_parameter.Value = JsonConvert.SerializeObject(request.specific_course);
            Console.WriteLine(courseid_parameter.Value);
            if (request.specific_filetype != null)
            {
                if (request.specific_filetype.Count == 0)
                    return new Search_Response(4);
                request.specific_filetype = request.specific_filetype.Distinct().ToList();
                foreach (var type in request.specific_filetype)
                {
                    if (!File_Process_Tool.File_Allowed(type))
                        return new Search_Response(4);
                }
                where_condition.Append(" AND ");
                where_condition.Append("[File_Typename] IN (select value from openjson(@FileTypeValue))");
                filetype_parameter.Value = JsonConvert.SerializeObject(request.specific_filetype);
            }
            if (request.should_before != default(DateTime))
            {
                if (Invalid_Datetime(request.should_before))
                    return new Search_Response(5);
                where_condition.Append("[Upload_Time] <= @ShouldBeforeValue");
                shouldbefore_parameter.Value = request.should_before;
            }
            if (request.should_after != default(DateTime))
            {
                if (Invalid_Datetime(request.should_after))
                    return new Search_Response(5);
                where_condition.Append("[Upload_Time] >= @ShouldAfterValue");
                shouldafter_parameter.Value = request.should_after;
            }
            string orderby_str = "";
            if (request.order_by == 0)
            {
                if (request.order == 0)
                    orderby_str = " ORDER BY [Upload_Time] DESC";
                else
                    orderby_str = " ORDER BY [Upload_Time] ASC";
            }
            else
            {
                if (request.order == 0)
                    orderby_str = " ORDER BY [Likes] DESC";
                else
                    orderby_str = " ORDER BY [Likes] ASC";
            }
            request.next_20 *= 20;
            IQueryable<UploadedFile> database_result = uploadedFileContext.UploadedFiles.FromSql("SELECT * FROM [CourseZero].[dbo].[UploadedFiles] " +  where_condition.ToString()  + orderby_str + " OFFSET @next_20 ROWS FETCH NEXT 20 ROWS ONLY;",
                 new SqlParameter("next_20", request.next_20), courseid_parameter, filetype_parameter, shouldbefore_parameter, shouldafter_parameter);
            foreach (var result in database_result)
                Add_Result_To_Response(result, response_result);
            response.result = response_result;
            return response;
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
        public async Task<ActionResult<Search_Response>> SearchByQuery([FromBody]SearchByQuery_Request request)
        {
            var response = new Search_Response();
            var response_result = new List<File_Shown_to_User>();
            request.search_query = request.search_query.Trim();
            if (request.search_query.Length < 4)
                return new Search_Response(2);
            StringBuilder where_condition = new StringBuilder();
            where_condition.Append("WHERE ");
            SqlParameter courseid_parameter = new SqlParameter("CourseIDValue", "");
            SqlParameter filetype_parameter = new SqlParameter("FileTypeValue", "");
            SqlParameter shouldbefore_parameter = new SqlParameter("ShouldBeforeValue", "");
            SqlParameter shouldafter_parameter = new SqlParameter("ShouldAfterValue", "");
            int where_count = 0;
            if (request.specific_course != null)
            {
                if (request.specific_course.Count >= 10 || request.specific_course.Count == 0)
                    return new Search_Response(3);

                if (Invalid_CourseID(request.specific_course))
                    return new Search_Response(3);
                where_count++;
                if (where_count > 1)
                    where_condition.Append(" AND ");
                where_condition.Append("[Related_courseID] IN (select value from openjson(@CourseIDValue))");
                courseid_parameter.Value = JsonConvert.SerializeObject(request.specific_course);
            }
            if (request.specific_filetype != null)
            {
                if (request.specific_filetype.Count == 0)
                    return new Search_Response(4);
                request.specific_filetype = request.specific_filetype.Distinct().ToList();
                foreach (var type in request.specific_filetype)
                {
                    if (!File_Process_Tool.File_Allowed(type))
                        return new Search_Response(4);
                    where_count++;
                    if (where_count > 1)
                        where_condition.Append(" AND ");
                    where_condition.Append("[File_Typename] IN (select value from openjson(@FileTypeValue))");
                    filetype_parameter.Value = JsonConvert.SerializeObject(request.specific_filetype);
                }
            }
            if (request.should_before != default(DateTime))
            {
                if (Invalid_Datetime(request.should_before))
                    return new Search_Response(5);
                where_count++;
                if (where_count > 1)
                    where_condition.Append(" AND ");
                where_condition.Append("[Upload_Time] <= @ShouldBeforeValue");
                shouldbefore_parameter.Value = request.should_before;
            }
            if (request.should_after != default(DateTime))
            {
                if (Invalid_Datetime(request.should_after))
                    return new Search_Response(5);
                where_count++;
                if (where_count > 1)
                    where_condition.Append(" AND ");
                where_condition.Append("[Upload_Time] >= @ShouldAfterValue");
                shouldafter_parameter.Value = request.should_after;
            }
            request.next_20 *= 20;
            IQueryable<UploadedFile> database_result = uploadedFileContext.UploadedFiles.FromSql("SELECT ftt.[RANK], v.* FROM FREETEXTTABLE ([CourseZero].[dbo].[UploadedFiles], ([Binary], [Words_for_Search]), @search_query) ftt INNER JOIN [CourseZero].[dbo].[UploadedFiles] v ON v.ID = ftt.[KEY] " + ((where_count > 0) ? where_condition.ToString() : "") + " ORDER BY ftt.[RANK] DESC OFFSET @next_20 ROWS FETCH NEXT 20 ROWS ONLY;",
                new SqlParameter("search_query", request.search_query), new SqlParameter("next_20", request.next_20), courseid_parameter, filetype_parameter, shouldbefore_parameter, shouldafter_parameter);
            foreach (var result in database_result)
                Add_Result_To_Response(result, response_result);
            response.result = response_result;
            return response;

        }
        private bool Invalid_CourseID(List<int> Courseids)
        {
            foreach (var courseid in Courseids)
            {
                if (courseid < CUSIS_Fetch_Service.CourseID_range.lower || courseid > CUSIS_Fetch_Service.CourseID_range.upper)
                    return true;
            }
            return false;
        }
        private bool Invalid_Datetime(DateTime dateTime)
        {
            return (DateTime.Compare(dateTime, (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue) < 0 || DateTime.Compare(dateTime, (DateTime)System.Data.SqlTypes.SqlDateTime.MaxValue) > 0);
        }

        private void Add_Result_To_Response(UploadedFile result, List<File_Shown_to_User> list)
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
            list.Add(file_Shown_To_User);
        }
        public class Search_Response
        {
            public Search_Response()
            {

            }
            public Search_Response(int status_code)
            {
                this.status_code = status_code;
            }
            public int status_code { get; set; }
            public List<File_Shown_to_User> result { get; set; }
        }
        public class SearchByUserID_Request
        {
            [Required]
            [StringLength(128, MinimumLength = 128)]
            public string auth_token { get; set; }
            /// <summary>
            /// <para>The user id that the files from</para>
            /// </summary>
            [Required]
            [Range(1, int.MaxValue)]
            public int userid { get; set; }
            /// <summary>
            /// 0 = by upload time, 1 = by likes
            /// </summary>
            [Required]
            public int order_by { get; set; }
            /// <summary>
            /// 0 = decreasing order, 1 = increasing order
            /// </summary>
            [Required]
            public int order { get; set; }
            public List<string> specific_filetype { get; set; }
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
            /// first 20 queries = 0, next 20 queries = 1, next next 20 queries = 2, etc.
            /// </summary>
            [Required]
            [Range(0, int.MaxValue)]

            public int next_20 { get; set; }

        }
        public class SearchByCourseID_Request
        {
            [Required]
            [StringLength(128, MinimumLength = 128)]
            public string auth_token { get; set; }
            /// <summary>
            /// <para>List of targeted courseID</para>
            /// <para>Maximum specific course is 10, otherwise status_code = 3</para>
            /// </summary>
            [Required]
            public List<int> specific_course { get; set; }
            /// <summary>
            /// 0 = by upload time, 1 = by likes
            /// </summary>
            [Required]
            public int order_by { get; set; }
            /// <summary>
            /// 0 = decreasing order, 1 = increasing order
            /// </summary>
            [Required]
            public int order { get; set; }
            public List<string> specific_filetype { get; set; }
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
            /// first 20 queries = 0, next 20 queries = 1, next next 20 queries = 2, etc.
            /// </summary>
            [Required]
            [Range(0, int.MaxValue)]

            public int next_20 { get; set; }

        }
        public class SearchByQuery_Request
        {
            [Required]
            [StringLength(128, MinimumLength = 128)]
            public string auth_token { get; set; }
            /// <summary>
            /// <para>text which the user typed in the search box</para>
            /// <para>the query has to contain at least 4 letters, otherwise status_code = 2</para>
            /// </summary>
            [Required]
            [StringLength(256, MinimumLength = 4)]
            public string search_query { get; set; }
            /// <summary>
            /// <para>List of targeted courseID</para>
            /// <para>Maximum specific course is 10 and values are valid, otherwise status_code = 3</para>
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