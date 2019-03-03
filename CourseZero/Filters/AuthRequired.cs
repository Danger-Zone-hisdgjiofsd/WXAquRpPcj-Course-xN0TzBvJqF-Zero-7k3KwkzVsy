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
        readonly AllDbContext allDbContext;
        public AuthRequired(AllDbContext allDbContext)
        {
            this.allDbContext = allDbContext;
        }
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var jObject = JObject.FromObject(context.ActionArguments.First().Value );
            string auth_token = jObject["auth_token"].ToObject<string>();
            AuthToken current_token = await allDbContext.AuthTokens.FirstOrDefaultAsync(x => x.Token == auth_token);
            if (current_token == null)
            {
                context.Result = new JsonResult(new { status_code = 1 });
            }
            else
                await next();
        }
    }
}
