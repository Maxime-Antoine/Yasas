using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenIddict;
using OpenIddict.Models;
using OpenIddict.EntityFrameworkCore;
using OpenIddict.Core;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Yasas.API
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer("");
                options.UseOpenIddict();
            });

            services.AddIdentity<AppUser, IdentityRole>()
                    .AddUserStore<AppUser>()
                    .AddRoleStore<IdentityRole>()
                    .AddDefaultTokenProviders();

            services.AddOpenIddict()
                    .AddEntityFrameworkCoreStores<AppDbContext>()
                    .EnableTokenEndpoint("/connect/token")
                    .EnableLogoutEndpoint("/connect/logout")
                    .AllowPasswordFlow()
                    .AllowRefreshTokenFlow()
                    .Configure(config => {
                        config.ApplicationCanDisplayErrors = true;
                    })
                    .DisableHttpsRequirement();                    
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello World!");
            //});

            app.UseIdentity();

            app.UseOAuthValidation();

            app.UseOpenIddict(); //needs to be after UseIdentity()

            app.UseMvcWithDefaultRoute();
        }
    }
}
