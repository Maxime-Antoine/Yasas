using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Yasas.API
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
    }

    public class AppUser : IdentityUser
    { }
}
