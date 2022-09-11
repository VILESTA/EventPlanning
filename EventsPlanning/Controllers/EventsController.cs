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
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Encodings.Web;

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
        [HttpGet]
        public ActionResult Create()
        {
            return View(new Event());
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public ActionResult Create(Event _event, List<string> names, List<string> values)
        {
            if(_event != null)
            {
                _event.AuthorId = User.Identity.GetUserId();
                List<AdditionalField> fields = new List<AdditionalField>();
                for (int i = 0; i < names.Count; i++)
                {
                    fields.Add(new AdditionalField() { Id = Guid.NewGuid().ToString(), Name = names[i], Value = values[i] });
                }
                List<EventFields> eventFields = new List<EventFields>();
                foreach (AdditionalField field in fields)
                {
                    eventFields.Add(new EventFields() { EventId = _event.EventId, FieldId = field.Id });
                }
                bool result = EventManager.Add(_event, fields, eventFields);
                if (result)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Произошла неизвестная ошибка при добавлении мероприятия");
                    ModelState.AddModelError(string.Empty, _event.ToString());
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
            if (eventId == null)
            {
                return View("Error");
            }
            Event cur_event = EventManager.Events.First(e => e.EventId == eventId);
            ViewBag.CountOfMembers = EventManager.CountOfMembersOfEvent(cur_event).ToString();
            ViewBag.Author = EventManager.AuthorOfEvent(cur_event);
            ViewBag.IsAuthor = String.Compare(EventManager.AuthorIDOfEvent(cur_event), User.Identity.GetUserId());
            return View(cur_event);
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public ActionResult Delete(string eventId)
        {
            if (eventId == null)
            {
                return View("Error");
            }
            Event cur_event = EventManager.Events.First(e => e.EventId == eventId);
            bool result = EventManager.Delete(cur_event);
            if (result)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Произошла неизвестная ошибка при удалении мероприятия");
            }
            return View("Event", cur_event);
        }

        [HttpGet]
        public ActionResult SignUpForEvent(string eventId)
        {
            if (User.Identity.GetUserId() == null)
            {
                return View("Error");
            }
            if (eventId == null)
            {
                return View("Error");
            }
            Event cur_event = EventManager.Events.First(e => e.EventId == eventId);
            EventManager.SignUpUser(cur_event, User.Identity.GetUserId());
            return RedirectToAction("Event", routeValues: new { eventId = cur_event.EventId });
        }

        [HttpGet]
        public ActionResult SignOutFromEvent(string eventId)
        {
            if (User.Identity.GetUserId() == null)
            {
                return View("Error");
            }
            if (eventId == null)
            {
                return View("Error");
            }
            Event cur_event = EventManager.Events.First(e => e.EventId == eventId);
            EventManager.SignOutUser(cur_event, User.Identity.GetUserId());
            return RedirectToAction("Event", routeValues: new { eventId = cur_event.EventId });
        }

        public bool IsUserOnEvent(string eventId)
        {
            if (User.Identity.GetUserId() == null)
            {
                return false;
            }
            if (eventId == null)
            {
                return false;
            }
            Event cur_event = EventManager.Events.First(e => e.EventId == eventId);
            if(EventManager.IsUserOnEvent(cur_event, User.Identity.GetUserId()))
            {
                return true;
            }
            return false;
        }

        public bool IsUserACreatorOfEvent(string eventId)
        {
            if (eventId == null)
            {
                return false;
            }
            Event cur_event = null;
            foreach (var item in EventManager.Events)
            {
                if(item.EventId == eventId)
                {
                    cur_event = item;
                }
            }
            ViewBag.EventData = cur_event.AuthorId;
            ViewBag.AuthorID = User.Identity.GetUserId();
            return EventManager.IsUserOnEvent(cur_event, User.Identity.GetUserId());
        }

        public int CountOfUsersOnEvent(string eventId)
        {
            return EventManager.CountOfMembersOfEvent(EventManager.Events.FirstOrDefault(e => e.EventId == eventId));
        }

        [Authorize(Roles = "admin")]
        public ActionResult Edit()
        {
            return View();
        }
    }
}