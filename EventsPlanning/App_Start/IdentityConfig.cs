using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using EventsPlanning.Models;
using System.Diagnostics;
using System.Web.DynamicData;
using System.Collections.ObjectModel;

namespace EventsPlanning
{
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Подключите здесь службу почты для отправки сообщения.
            return Task.FromResult(0);
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Подключите здесь службу SMS, чтобы отправить текстовое сообщение.
            return Task.FromResult(0);
        }
    }

    // Настройка диспетчера пользователей приложения. UserManager определяется в ASP.NET Identity и используется приложением.
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store) : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context) 
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            // Настройка логики проверки имен пользователей
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Настройка логики проверки паролей
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 8,
                RequireNonLetterOrDigit = false,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            // Настройка параметров блокировки по умолчанию
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Регистрация поставщиков двухфакторной проверки подлинности. Для получения кода проверки пользователя в данном приложении используется телефон и сообщения.
            // Здесь можно указать собственный поставщик и подключить его.
            manager.RegisterTwoFactorProvider("Код, полученный по телефону", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Ваш код безопасности: {0}"
            });
            manager.RegisterTwoFactorProvider("Код из сообщения", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "Код безопасности",
                BodyFormat = "Ваш код безопасности: {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = 
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

    // Настройка диспетчера входа для приложения.
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager) : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }

    public class ApplicationRoleManager : RoleManager<IdentityRole>
    {
        public ApplicationRoleManager(IRoleStore<IdentityRole, string> store) : base(store)
        {
        }

        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            return new ApplicationRoleManager(new RoleStore<IdentityRole>(context.Get<ApplicationDbContext>()));
        }
    }

    public class ApplicationEventManager : IDisposable
    {
        public List<Event> Events { get; set; }
        public static IOwinContext context { get; set; }

        public ApplicationEventManager(List<Event> events)
        {
            Events = events;
        }

        public static ApplicationEventManager Create(IdentityFactoryOptions<ApplicationEventManager> options, IOwinContext _context)
        {
            context = _context;
            return new ApplicationEventManager(_context.Get<ApplicationEventDbContext>().Events.ToList());
        }

        public bool Add(Event _event)
        {
            try
            {
                Events.Add(_event);
                context.Get<ApplicationEventDbContext>().Entry(_event).State = EntityState.Added;
                context.Get<ApplicationEventDbContext>().SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool SignUpUser(Event _event, string userId)
        {
            try
            {
                int membersInEvent = 0;
                foreach (var member in context.Get<ApplicationEventDbContext>().EventsUsers)
                {
                    if (member.EventId == _event.EventId)
                    {
                        membersInEvent++;
                    }
                }
                if (membersInEvent >= _event.MaxMembersCount)
                {
                    return false;
                }
                context.Get<ApplicationEventDbContext>().Entry(new EventUsers(_event.EventId, userId)).State = EntityState.Added;
                context.Get<ApplicationEventDbContext>().SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool SignOutUser(Event _event, string userId)
        {
            try
            {
                EventUsers eventUsers = context.Get<ApplicationEventDbContext>().EventsUsers.First(e => e.EventId == _event.EventId && e.UserId == userId);
                context.Get<ApplicationEventDbContext>().Entry(eventUsers).State = EntityState.Deleted;
                context.Get<ApplicationEventDbContext>().SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool IsUserOnEvent(Event _event, string userId)
        {
            try
            {

                EventUsers eventUsers = context.Get<ApplicationEventDbContext>().EventsUsers.First(e => e.EventId == _event.EventId && e.UserId == userId);
                if(eventUsers != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        
        public bool IsUserACreatorOfEvent(Event _event, string userId)
        {
            try
            {
                Event usersEvent = context.Get<ApplicationEventDbContext>().Events.First(e => e.EventId == _event.EventId && e.AuthorId == userId);
                if(usersEvent != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public int CountOfMembersOfEvent(Event _event)
        {
            int count = 0;
            List<EventUsers> eventUsers = context.Get<ApplicationEventDbContext>().EventsUsers.ToList();
            if (eventUsers != null)
            {
                foreach (var item in eventUsers)
                {
                    if(item.EventId == _event.EventId)
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        public string AuthorOfEvent(Event _event)
        {
            ApplicationUserManager users = context.GetUserManager<ApplicationUserManager>();
            if (users != null)
            {
                foreach (var item in users.Users)
                {
                    if (item.Id == _event.AuthorId)
                    {
                        return item.UserName;
                    }
                }
            }
            return "Автор не найден";
        }

        public void Dispose()
        {
        }
    }
}
