using EventsPlanning.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;

namespace EventsPlanning.Controllers
{
    public class RolesController : Controller
    {
        public RolesController()
        {
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }

        public ApplicationRoleManager RoleManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<ApplicationRoleManager>();
            }
        }

        [Authorize(Roles = "admin")]
        public ActionResult Index() => View(RoleManager.Roles.ToList());

        public ActionResult Create() => View();
        [HttpPost]
        public async Task<ActionResult> Create(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                IdentityResult result = await RoleManager.CreateAsync(new IdentityRole(name));
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }
                }
            }
            return View(name);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<ActionResult> Delete(string id)
        {
            IdentityRole role = await RoleManager.FindByIdAsync(id);
            if (role != null)
            {
                IdentityResult result = await RoleManager.DeleteAsync(role);
            }
            return RedirectToAction("Index");
        }

        public ActionResult UserList() => View(UserManager.Users.ToList());


        [HttpGet]
        public ActionResult Edit(string userId)
        {
            // получаем пользователя
            ApplicationUser user = UserManager.FindByIdAsync(userId).Result;
            if (user != null)
            {
                // получем список ролей пользователя
                var userRoles = UserManager.GetRolesAsync(user.Id).Result;
                var allRoles = RoleManager.Roles.ToList();
                ChangeRoleViewModel model = new ChangeRoleViewModel
                {
                    UserId = user.Id,
                    UserEmail = user.Email,
                    UserRoles = userRoles,
                    AllRoles = allRoles
                };
                return View(model);
            }

            return View("Error");
        }


        [HttpPost]
        public ActionResult Edit(string userId, List<string> roles)
        {
            // получаем пользователя
            ApplicationUser user = UserManager.FindByIdAsync(userId).Result;
            if (user != null)
            {
                // получем список ролей пользователя
                var userRoles = UserManager.GetRolesAsync(user.Id).Result;
                // получаем все роли
                var allRoles = RoleManager.Roles.ToList();
                // получаем список ролей, которые были добавлены
                var addedRoles = roles.Except(userRoles);
                // получаем роли, которые были удалены
                var removedRoles = userRoles.Except(roles);

                UserManager.AddToRolesAsync(user.Id, addedRoles.ToArray()).Wait();

                UserManager.RemoveFromRolesAsync(user.Id, removedRoles.ToArray()).Wait();

                return RedirectToAction("UserList");
            }

            return View("Error");
        }
    }
}