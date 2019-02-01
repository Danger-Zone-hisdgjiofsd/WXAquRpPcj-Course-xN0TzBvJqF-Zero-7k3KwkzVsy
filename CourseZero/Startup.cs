using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CourseZero.Controllers;
using CourseZero.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace CourseZero
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        private readonly IConfiguration _config;
        public Startup(IConfiguration config)
        {
            _config = config;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddSwaggerGen(x =>
            {
                var info = new Swashbuckle.AspNetCore.Swagger.Info();
                info.Title = "API Reference";
                info.Version = DateTime.Now.ToString();
                x.SwaggerDoc("v1", info);
                x.IncludeXmlComments(AppDomain.CurrentDomain.BaseDirectory + "Doc.xml");
            });
            services.AddMvc(x =>
            {
                x.Filters.Add(typeof(ValidatorActionFilter));
            });
            services.AddDbContext<UserContext>(options =>
            {
                options.UseSqlServer(_config.GetConnectionString("DefaultConnection"));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime Lifetime)
        {
            app.UseStaticFiles();
            app.UseMvc();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(x =>
                {
                    x.SwaggerEndpoint("/swagger/v1/swagger.json", "API Reference");
                });
            }
            Lifetime.ApplicationStarted.Register(() =>
            {
               //Application start
            });

            Lifetime.ApplicationStopping.Register(() =>
            {
                //Application stopping
            });


            /*app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World\r\n");
            });*/

           
        }
    }
}
