using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AccountManager.WebApi.Controllers
{
    public class HomeController : Controller
    {
        public void Index()
        {
            ViewBag.Title = "Home Page";
            Response.Write("Accout Manager v2 / Updater v3");
           // return View();
        }
    }
}
