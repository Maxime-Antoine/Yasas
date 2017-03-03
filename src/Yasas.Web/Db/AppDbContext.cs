using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Yasas.Web.Models;

namespace Yasas.Web.Db
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        private static object _migrationLock = new object();
        private static bool _migrated = false;

        public AppDbContext(DbContextOptions options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}