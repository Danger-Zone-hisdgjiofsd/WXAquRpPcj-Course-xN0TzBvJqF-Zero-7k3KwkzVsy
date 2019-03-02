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
using CourseZero.Filters;
using Microsoft.Extensions.Hosting;
using CourseZero.Services;

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
            services.AddSingleton<IHostedService, CUSIS_Fetch_Service>();
            services.AddSingleton<IHostedService, File_Process_Service>();
            services.AddMvc();
            services.AddSwaggerGen(x =>
            {
                var info = new Swashbuckle.AspNetCore.Swagger.Info();
                info.Title = "API Reference";
                info.Version = DateTime.Now.ToString();
                x.SwaggerDoc("v1", info);
                x.IncludeXmlComments(AppDomain.CurrentDomain.BaseDirectory + "CourseZero.xml");
            });
            services.AddMvc(x =>
            {
                x.Filters.Add(typeof(ValidatorActionFilter));
            });
            services.AddScoped<AuthRequired>();
            services.AddDbContext<UserContext>(options =>
            {
                options.UseSqlServer(_config.GetConnectionString("DefaultConnection"));
            });
            services.AddDbContext<AuthTokenContext>(options =>
            {
                options.UseSqlServer(_config.GetConnectionString("DefaultConnection"));
            });
            services.AddDbContext<UploadHistContext>(options =>
            {
                options.UseSqlServer(_config.GetConnectionString("DefaultConnection"));
            });
            services.AddDbContext<CourseContext>(options =>
            {
                options.UseSqlServer(_config.GetConnectionString("DefaultConnection"));
            });
            services.AddDbContext<UploadedFileContext>(options =>
            {
                options.UseSqlServer(_config.GetConnectionString("DefaultConnection"));
            });
            services.AddDbContext<SubscriptionContext>(options =>
            {
                options.UseSqlServer(_config.GetConnectionString("DefaultConnection"));
            });
            services.AddDbContext<WatchLaterContext>(options =>
            {
                options.UseSqlServer(_config.GetConnectionString("DefaultConnection"));
            });
            services.AddDbContext<ProfileCommentsContext>(options =>
            {
                options.UseSqlServer(_config.GetConnectionString("DefaultConnection"));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, Microsoft.AspNetCore.Hosting.IApplicationLifetime Lifetime)
        {
            app.UseDefaultFiles(new DefaultFilesOptions
            {
                DefaultFileNames = new List<string> { "newmainpage.html" }
            });
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
