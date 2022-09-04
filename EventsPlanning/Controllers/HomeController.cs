using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EventsPlanning.Controllers
{
    public class HomeController : Controller
    {
        public ApplicationEventManager EventManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<ApplicationEventManager>();
            }
        }

        public ActionResult Index()
        {
            ViewBag.Message = "Здесь вы можете найти мероприятия, которые будут вам интересны";
            return View(EventManager.Events);
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