using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EventsPlanning.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Какая-то информация о сайте";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Здесь контактные данные с создателем";

            return View();
        }
    }
}