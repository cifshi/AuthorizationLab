using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace AuthorizationLab
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
             .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
              options =>
        {
            options.LoginPath = new PathString("/Account/Login/");
            options.AccessDeniedPath = new PathString("/Account/Forbidden/");
        });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdministratorOnly", policy => policy.RequireRole("Administrator"));
                options.AddPolicy("EmployeeId", policy => policy.RequireClaim("EmployeeId","123","456"));
                options.AddPolicy("Over21Only", policy => policy.Requirements.Add(new MinimumAgeRequirement(21, SomeTypes.AAA)));
            });

            services.AddMvc(config => {

                var policy = new AuthorizationPolicyBuilder()
                     .RequireAuthenticatedUser()
                     .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //};
            app.UseAuthentication();            
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync($"<h1 style='color:blue'>Hello : {Environment.MachineName}{DateTime.Now}</h1>");
            //    // WaitASecond();
            //    await context.Response.WriteAsync("<h1 style='color:green'>" + "Hello : " + Environment.MachineName + DateTime.Now + "</h1>");
            //});

           
        }
        //private static void WaitASecond()
        //{
        //    System.Threading.Thread.Sleep(millisecondsTimeout: 1000);
        //}
    }
}
