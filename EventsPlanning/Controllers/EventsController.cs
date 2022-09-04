using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using EventsPlanning.Models;
using Microsoft.Owin.BuilderProperties;
using System.Data.Entity.Spatial;

namespace EventsPlanning.Controllers
{
    public class EventsController : Controller
    {
        public ApplicationEventManager EventManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<ApplicationEventManager>();
            }
        }

        [Authorize(Roles = "admin")]
        public ActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public ActionResult Create(string Title, string Address, DateTime DateTime)
        {
            if (!string.IsNullOrEmpty(Title) && !string.IsNullOrEmpty(Address) && !DateTime.Equals(null))
            {
                Event new_event = new Event(Title, Address, DateTime);
                bool result = EventManager.Add(new_event);
                if (result)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Произошла неизвестная ошибка при добавлении мероприятия");
                }
            }
            return View();
        }

        [Authorize(Roles = "admin")]
        public ActionResult Edit()
        {
            return View();
        }

        [Authorize(Roles = "admin")]
        public ActionResult Delete()
        {
            return View();
        }
    }
}