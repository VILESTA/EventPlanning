using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using EventPlanning.Data;
using Microsoft.AspNetCore.Html;
using System.Text.RegularExpressions;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity;

namespace EventPlanning.Pages.Account
{
    public class RegisterModel : PageModel
    {
        public ApplicationContext _context;
        [BindProperty]
        public new EventPlanning.User User { get; set; } = default!;

        public RegisterModel(ApplicationContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || _context.Users == null || User == null)
            {
                return Page();
            }
            Regex regex = new Regex(@"(\w*)@(\w*).(\w*)");
            if (regex.IsMatch(User.Email))
            {
                if(_context.Users.Count() == 0)
                {
                    if (User.Password != null && User.ConfirmPassword != null)
                    {
                        if (User.Password == User.ConfirmPassword && User.Password.Length >= 8 && User.Password.Length <= 32)
                        {
                            await _context.AddAsync(User);
                            await _context.SaveChangesAsync();
                            return RedirectToPage("/Account/Login");
                        }
                    }
                }
                else if (_context.Users.First(p => p.Email == User.Email) == null)
                {
                    if (User.Password != null && User.ConfirmPassword != null)
                    {
                        if (User.Password == User.ConfirmPassword && User.Password.Length >= 8 && User.Password.Length <= 32)
                        {
                            await _context.AddAsync(User);
                            await _context.SaveChangesAsync();

                            MailAddress from = new MailAddress("somemail@yandex.ru", "Web Registration");
                            MailAddress to = new MailAddress(User.Email);
                            MailMessage m = new MailMessage(from, to);
                            m.Subject = "Email confirmation";
                            m.Body = string.Format("Для завершения регистрации перейдите по ссылке:" +
                                            "<a href=\"{0}\" title=\"Подтвердить регистрацию\">{0}</a>",
                                Url.Action("ConfirmEmail", "Account", new { Token = User.Id, Email = User.Email }, Request.Path.Value));
                            m.IsBodyHtml = true;
                            // адрес smtp-сервера, с которого мы и будем отправлять письмо
                            SmtpClient smtp = new System.Net.Mail.SmtpClient("smtp.yandex.ru", 25);
                            // логин и пароль
                            smtp.Credentials = new System.Net.NetworkCredential("somemail@yandex.ru", "password");
                            smtp.Send(m);
                            return RedirectToAction("Confirm", "Account", new { Email = User.Email });
                        }
                    }
                }
            }
            return Page();
        }

        public string Confirm(string Email)
        {
            return "На почтовый адрес " + Email + " Вам высланы дальнейшие" +
                    "инструкции по завершению регистрации";
        }

        public async Task<ActionResult> ConfirmEmail(string Token, string Email)
        {
            ApplicationUser user = this.UserManager.FindById(Token);
            if (user != null)
            {
                if (user.Email == Email)
                {
                    user.ConfirmedEmail = true;
                    await UserManager.UpdateAsync(user);
                    await SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home", new { ConfirmedEmail = user.Email });
                }
                else
                {
                    return RedirectToAction("Confirm", "Account", new { Email = user.Email });
                }
            }
            else
            {
                return RedirectToAction("Confirm", "Account", new { Email = "" });
            }
        }
    }
}
