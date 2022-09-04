using System;
using System.Data.Entity;
using System.Data.Entity.Spatial;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.BuilderProperties;

namespace EventsPlanning.Models
{
    // В профиль пользователя можно добавить дополнительные данные, если указать больше свойств для класса ApplicationUser. Подробности см. на странице https://go.microsoft.com/fwlink/?LinkID=317594.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Обратите внимание, что authenticationType должен совпадать с типом, определенным в CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Здесь добавьте утверждения пользователя
            return userIdentity;
        }
    }

    public class Event
    {
        public string EventId { get; set; }
        public string Title { get; set; }
        public DateTime DateTime { get; set; }
        public string Address { get; set; }

        public Event()
        {
        }

        public Event(string title, string address, DateTime dateTime)
        { 
            EventId = Guid.NewGuid().ToString();
            Title = title;
            Address = address;
            DateTime = dateTime;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext() : base("DefaultConnection", throwIfV1Schema: false)
        {
            Database.CreateIfNotExists();
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }

    public class ApplicationEventDbContext : DbContext
    {
        public DbSet<Event> Events { get; set; }
        public ApplicationEventDbContext() : base("DefaultConnection")
        {
            Database.CreateIfNotExists();
        }

        public static ApplicationEventDbContext Create()
        {
            return new ApplicationEventDbContext();
        }
    }
}