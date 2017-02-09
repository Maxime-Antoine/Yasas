using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Yasas.API
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
    }

    public class AppUser : IdentityUser
    { }
}