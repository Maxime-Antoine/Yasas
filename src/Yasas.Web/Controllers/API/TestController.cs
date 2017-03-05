using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Yasas.Web.Controllers.API
{
    public class TestController : Controller
    {
        [Authorize(Roles = "Admin")]
        [Route("~/api/Admin")]
        public IActionResult Get()
        {
            return Ok("You're authenticated with admin role !");
        }
    }
}
