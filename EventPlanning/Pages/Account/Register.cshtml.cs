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
                if (_context.Users.First(p => p.Email == User.Email) == null)
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
            return Page();
        }
    }
}
