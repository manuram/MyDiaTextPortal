using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SeniorDesign.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult General(Exception exception)
        {
            ViewBag.errormessage = exception.Message;            
            return View("Error");
        }

        public ActionResult Http404()
        {
            ViewBag.errormessage = "404 - Page not found";
            return View("Error");
        }

        public ActionResult Http403()
        {
            ViewBag.errormessage = "403 - Forbidden";
            return View("Error");
        }
    }
}