using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Spatial;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNetCore.Mvc;
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

    public class AdditionalField
    {
        public string Id { get; set; }

        [BindProperty]
        public string Name { get; set; }

        [BindProperty]
        public string Value { get; set; }

        public AdditionalField()
        {
            Id = Guid.NewGuid().ToString();
        }
    }

    public class EventFields
    {
        [Key]
        [BindProperty]
        public string FieldId { get; set; }

        [BindProperty]
        public string EventId { get; set; }
    }

    public class Event
    {
        [BindProperty]
        public string EventId { get; set; }
        [BindProperty]
        public string AuthorId { get; set; }
        [BindProperty]
        public string Title { get; set; }

        [BindProperty]
        [DataType(DataType.Date)]
        public DateTime DateTime { get; set; }
        
        [BindProperty]
        public string Address { get; set; }

        [BindProperty]
        public int MaxMembersCount { get; set; } = 1;

        public Event()
        {
            EventId = Guid.NewGuid().ToString();
        }

        public Event(string title, string authorID, string address, DateTime dateTime, int maxMembersCount)
        { 
            EventId = Guid.NewGuid().ToString();
            AuthorId = authorID;
            Title = title;
            Address = address;
            DateTime = dateTime;
            MaxMembersCount = maxMembersCount;
        }

        public override string ToString()
        {
            return $"{EventId}\n{Title} [{AuthorId}]\n{Address} | {DateTime}\n{MaxMembersCount}";
        }
    }

    public class EventUsers
    {
        [Key]
        [BindProperty]
        public string EventId { get; set; }
        [BindProperty]
        public string UserId { get; set; }

        public EventUsers()
        {
        }

        public EventUsers(string eventId, string userId)
        {
            UserId = userId;
            EventId = eventId;
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
        public DbSet<EventUsers> EventsUsers { get; set; }
        public DbSet<AdditionalField> additionalFields { get; set; }
        public DbSet<EventFields> eventFields { get; set; }
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