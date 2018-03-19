using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Consultation.Controllers
{
    public class ErrorController : Controller
    {
        // GET: /<controller>/
        public IActionResult Authentication(string errorType, string errorDetal)
        {
            ViewData["errorType"] = errorType;
            ViewData["errorDetal"] = errorDetal;
            return View();
        }

        public IActionResult Building()
        {
            return View();
        }
    }
}
