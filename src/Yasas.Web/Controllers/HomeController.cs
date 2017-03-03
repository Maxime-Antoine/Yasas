using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Yasas.Web.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}