using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CourseZero.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Z.Linq;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CourseZero.Controllers
{
    [Route("api/[controller]")]
    public class SessionController : Controller
    {
        readonly UserContext userContext;
        readonly AuthTokenContext authTokenContext;
        public SessionController(UserContext userContext, AuthTokenContext authTokenContext)
        {
            this.userContext = userContext;
            this.authTokenContext = authTokenContext;
        }
        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [Route("[action]")]
        public async Task<ActionResult<Logout_Response>> Logout([FromBody]Logout_Request request)
        {
            var Response = new Logout_Response();
            AuthToken token = await authTokenContext.AuthTokens.FirstOrDefaultAsync(x => x.Token == request.auth_token);
            if (token == null)
            {
                Response.status_code = 1;
                return Response;
            }
            authTokenContext.AuthTokens.Remove(token);
            await authTokenContext.SaveChangesAsync();
            Response.status_code = 0;
            return Response;
        }
        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [Route("[action]")]
        public async Task<ActionResult<Get_All_Sessions_Response>> Get_All_Sessions([FromBody]Get_All_Sessions_Request request)
        {
            var Response = new Get_All_Sessions_Response();
            AuthToken current_token = await authTokenContext.AuthTokens.FirstOrDefaultAsync(x => x.Token == request.auth_token);
            if (current_token == null)
            {
                Response.status_code = 1;
                return Response;
            }
            Response.status_code = 0;
            var list_of_auth_tokens = await authTokenContext.AuthTokens.WhereAsync(x => x.userID == current_token.userID);
            Response.sessions = new List<AuthToken>();
            Response.sessions.AddRange(list_of_auth_tokens);
            return Response;
        }
        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [Route("[action]")]
        public async Task<ActionResult<Logout_Specific_Session_Response>> Logout_Specific_Session([FromBody] Logout_Specific_Session_Request request)
        {
            var Response = new Logout_Specific_Session_Response();
            AuthToken current_token = await authTokenContext.AuthTokens.FirstOrDefaultAsync(x => x.Token == request.auth_token);
            if (current_token == null)
            {
                Response.status_code = 1;
                return Response;
            }
            AuthToken token_to_be_removed = await authTokenContext.AuthTokens.FirstOrDefaultAsync(x => x.userID == current_token.userID && x.Token == request.token_to_be_removed);
            if (token_to_be_removed == null)
            {
                Response.status_code = 2;
                return Response;
            }
            authTokenContext.AuthTokens.Remove(token_to_be_removed);
            await authTokenContext.SaveChangesAsync();
            Response.status_code = 0;
            return Response;
        }
    }
    public class Logout_Specific_Session_Request
    {
        [Required]
        [StringLength(128, MinimumLength = 128)]
        public string auth_token { get; set; }
        /// <summary>
        /// <para>the token that the user want to log it out</para>
        /// <para>should prevent the user logout its own session
        /// this part is not checked on server side</para>
        /// </summary>
        [Required]
        [StringLength(128, MinimumLength = 128)]
        public string token_to_be_removed { get; set; }
    }
    public class Logout_Specific_Session_Response
    {
        /// <summary>
        /// <para>0 is success, 1, 2 is fail</para>
        /// <para>1 fail is due to invalid auth token (user has already logged out)</para>
        /// <para>2 fail is due to given session does not exist or not valid</para>
        /// </summary>
        public int status_code { get; set; }
    }

    public class Get_All_Sessions_Request
    {
        [Required]
        [StringLength(128, MinimumLength = 128)]
        public string auth_token { get; set; }
    }
    public class Get_All_Sessions_Response
    {
        /// <summary>
        /// <para>0 is success, 1 is fail</para>
        /// <para>fail is due to invalid auth token (user has already logged out)</para>
        /// </summary>
        public int status_code { get; set; }
        /// <summary>
        /// Return a list of session information. 
        /// </summary>
        public List<AuthToken> sessions { get; set; }
    }
    public class Logout_Request
    {
        [Required]
        [StringLength(128, MinimumLength = 128)]
        public string auth_token { get; set; }
    }
    public class Logout_Response
    {
        /// <summary>
        /// <para>0 is success, 1 is fail</para>
        /// <para>fail may due to invalid auth token (user has already logged out)</para>
        /// </summary>
        public int status_code { get; set; }
    }
}
