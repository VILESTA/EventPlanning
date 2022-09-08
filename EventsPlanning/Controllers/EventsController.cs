﻿using Microsoft.AspNet.Identity.EntityFramework;
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

        Event new_event { get; set; } = new Event();

        [Authorize(Roles = "admin")]
        [HttpGet]
        public ActionResult Create(int addFieldsCount = 0)
        {
            if (addFieldsCount < 0) addFieldsCount = 0;
            for (int i = 0; i < addFieldsCount; i++)
            {
                new_event.Fields.Add(new AdditionalField());
            }
            return View(new_event);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public ActionResult Create(string title, List<AdditionalField> addFields, string address, DateTime dateTime, int maxMembersCount)
        {
            if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(address) && !dateTime.Equals(null) && !maxMembersCount.Equals(null) && maxMembersCount >= 2)
            {
                Event _event = new Event(title, User.Identity.GetUserId(), address, dateTime, maxMembersCount);
                _event.Fields = addFields;
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
            return View("Event", cur_event);
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
            return View("Event", cur_event);
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



        [Authorize(Roles = "admin")]
        public ActionResult Edit()
        {
            return View();
        }
    }
}