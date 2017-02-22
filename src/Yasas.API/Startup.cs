using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Yasas.API
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; set; }

        public Startup(IHostingEnvironment env)
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore()
                    .AddJsonFormatters()
                    .AddViews()
                    .AddRazorViewEngine();

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(Configuration["ConnectionStrings:DefaultConnection"]);
                options.UseOpenIddict();
            });

            services.AddIdentity<AppUser, IdentityRole>()
                    .AddEntityFrameworkStores<AppDbContext>()
                    .AddDefaultTokenProviders();

            services.AddOpenIddict()
                    .AddEntityFrameworkCoreStores<AppDbContext>()
                    .AddMvcBinders()
                    .EnableTokenEndpoint("/connect/token")
                    .EnableLogoutEndpoint("/connect/logout")
                    .AllowPasswordFlow()
                    .AllowRefreshTokenFlow()
                    .Configure(config =>
                    {
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
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello World!");
            //});

            app.UseIdentity();

            app.UseOAuthValidation();

            app.UseOpenIddict(); //needs to be after UseIdentity()

            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute(
            //        name: "default",
            //        template: "{controller=Home}/{action=Index}/{id?}");
            //});

            app.UseMvcWithDefaultRoute();

            app.UseStaticFiles();
        }
    }
}