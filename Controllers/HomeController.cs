using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Demiady.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult login(string pass)
        {
            string password = System.Configuration.ConfigurationManager.AppSettings["password"].ToString();
            if (pass == password)
            {

                ViewBag.error = "tr";
                return RedirectToAction("Index");

            }
            else
            {
                ViewBag.error = "fa";
                return View();
            }

        }
    }
}