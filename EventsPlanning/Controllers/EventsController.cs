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
using Microsoft.Extensions.Logging;

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

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
        }

        [Authorize(Roles = "admin")]
        public ActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public ActionResult Create(string Title, string Address, DateTime DateTime, int MaxMembersCount)
        {
            if (!string.IsNullOrEmpty(Title) && !string.IsNullOrEmpty(Address) && !DateTime.Equals(null) && !MaxMembersCount.Equals(null) && MaxMembersCount >= 2)
            {
                Event new_event = new Event(Title, User.Identity.GetUserId(), Address, DateTime, MaxMembersCount);
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
            else
            {
                ModelState.AddModelError(string.Empty, "Некоторые данные не были введены");
            }
            return View();
        }

        [HttpGet]
        public ActionResult Event(string eventId)
        {
            if(eventId == null)
            {
                return View("Error");
            }
            Event cur_event = EventManager.Events.First(e => e.EventId == eventId);
            return View(cur_event);
        }

        [HttpGet]
        public ActionResult SignUpForEvent(string eventId)
        {
            var user = SignInManager.UserManager.FindById(User.Identity.GetUserId());
            if (user == null)
            {
                return View("Error");
            }
            if (eventId == null)
            {
                return View("Error");
            }
            Event cur_event = EventManager.Events.First(e => e.EventId == eventId);
            EventManager.SignUpUser(cur_event, user.Id);
            return View("Event", cur_event);
        }

        [HttpGet]
        public ActionResult SignOutFromEvent(string eventId)
        {
            var user = SignInManager.UserManager.FindById(User.Identity.GetUserId());
            if (user == null)
            {
                return View("Error");
            }
            if (eventId == null)
            {
                return View("Error");
            }
            Event cur_event = EventManager.Events.First(e => e.EventId == eventId);
            EventManager.SignOutUser(cur_event, user.Id);
            return View("Event", cur_event);
        }

        public bool IsUserOnEvent(string eventId)
        {
            var user = SignInManager.UserManager.FindById(User.Identity.GetUserId());
            if (user == null)
            {
                return false;
            }
            if (eventId == null)
            {
                return false;
            }
            Event cur_event = EventManager.Events.First(e => e.EventId == eventId);
            if(EventManager.IsUserOnEvent(cur_event, user.Id))
            {
                return true;
            }
            return false;
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