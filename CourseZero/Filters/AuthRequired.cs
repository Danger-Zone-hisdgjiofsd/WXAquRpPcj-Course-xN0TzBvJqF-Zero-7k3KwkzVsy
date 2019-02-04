using CourseZero.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CourseZero.Filters
{
    public class AuthRequired : ActionFilterAttribute
    {
        readonly AuthTokenContext authTokenContext;
        public AuthRequired(AuthTokenContext authTokenContext)
        {
            this.authTokenContext = authTokenContext;
        }
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            foreach (var s in context.ActionArguments)
                Console.WriteLine(s);
            var jObject = JObject.FromObject(context.ActionArguments["request"] );
            foreach (var s in jObject.Properties().Select(p => p.Name).ToList())
                Console.WriteLine(s);
            string auth_token = jObject["auth_token"].ToObject<string>();
            AuthToken current_token = await authTokenContext.AuthTokens.FirstOrDefaultAsync(x => x.Token == auth_token);
            if (current_token == null)
            {
                context.Result = new JsonResult(new { status_code = 1 });
            }
            else
                await next();
        }
    }
}
