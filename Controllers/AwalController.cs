using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Linq.Dynamic;
using System.Linq.Dynamic.Core;


namespace mystap.Controllers
{
    public class AwalController : Controller
    {

        public IActionResult Awal()
        {
            return View();
        }

    }
}
