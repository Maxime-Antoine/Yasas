using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Yasas.Web.Db;
using Yasas.Web.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;

namespace Yasas.Web
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; private set; }
        public IHostingEnvironment Environment { get; private set; }

        public Startup(IHostingEnvironment env)
        {
            Environment = env;

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
                    .AddRazorViewEngine()
                    .AddAuthorization();

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(Configuration["ConnectionStrings:DefaultConnection"]);
                options.UseOpenIddict();
            });

            services.AddIdentity<AppUser, IdentityRole>()
                    .AddEntityFrameworkStores<AppDbContext>()
                    .AddDefaultTokenProviders();

            var signinKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["SIGNINGKEY"]));

            if (Environment.IsDevelopment())
            {
                services.AddOpenIddict()
                        .AddEntityFrameworkCoreStores<AppDbContext>()
                        .AddMvcBinders()
                        .EnableTokenEndpoint("/connect/token")
                        .EnableLogoutEndpoint("/connect/logout")
                        .AllowPasswordFlow()
                        .AllowRefreshTokenFlow()
                        .UseJsonWebTokens()
                        .AddSigningKey(signinKey)
                        .Configure(config =>
                        {
                            config.ApplicationCanDisplayErrors = true;
                        })
                        .DisableHttpsRequirement();
            }
            else
            {
                services.AddOpenIddict()
                        .AddEntityFrameworkCoreStores<AppDbContext>()
                        .AddMvcBinders()
                        .EnableTokenEndpoint("/connect/token")
                        .EnableLogoutEndpoint("/connect/logout")
                        .AllowPasswordFlow()
                        .AllowRefreshTokenFlow()
                        .UseJsonWebTokens()
                        .AddSigningKey(signinKey);
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseExceptionHandler("/Home/Error");

            app.UseJwtBearerAuthentication(new JwtBearerOptions {
                   AutomaticAuthenticate = true,
                   TokenValidationParameters = new TokenValidationParameters
                   {
                       IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["SIGNINGKEY"])),
                       ValidateIssuer = true,
                       ValidIssuer = Configuration["URL"],
                       ValidateAudience = true,
                       ValidAudience = Configuration["URL"]
                   }
               })
               .UseOpenIddict()
               .UseMvcWithDefaultRoute()
               .UseStaticFiles();
            
            //create & seed database
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var db = serviceScope.ServiceProvider.GetService<AppDbContext>();
                db.Database.EnsureDeleted(); //DEBUG
                db.Database.EnsureCreated();
                _SeedIdentityAsync(app.ApplicationServices).Wait();
            }
        }

        private async Task _SeedIdentityAsync(IServiceProvider serviceProvider)
        {
            var db = serviceProvider.GetRequiredService<AppDbContext>();
            var roleStore = new RoleStore<IdentityRole>(db);
            var userStore = new UserStore<AppUser>(db);

            if (!roleStore.Roles.Any(r => r.Name == "Admin"))
            {
                await roleStore.CreateAsync(new IdentityRole() { Name = "Admin", NormalizedName = "ADMIN" });
                await roleStore.CreateAsync(new IdentityRole() { Name = "User", NormalizedName = "USER" });
            }

            var user = new AppUser
            {
                FirstName = "Super",
                LastName = "User",
                UserName = "Admin",
                NormalizedUserName = "ADMIN",
                Email = "test@test.com",
                NormalizedEmail = "TEST@TEST.com",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString("D")
            };

            if(!userStore.Users.Any(u => u.UserName == user.UserName))
            {
                var hasher = new PasswordHasher<AppUser>();
                var hashedPassword = hasher.HashPassword(user, "Test123");
                user.PasswordHash = hashedPassword;

                await userStore.CreateAsync(user);

                var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
                var dbUser = await userManager.FindByNameAsync(user.UserName);
                await userManager.AddToRoleAsync(dbUser, "Admin");
                await userManager.AddToRoleAsync(dbUser, "User");

                await userManager.AddClaimAsync(dbUser, new Claim("sub", dbUser.UserName));
            }

            await db.SaveChangesAsync();
        }
    }
}