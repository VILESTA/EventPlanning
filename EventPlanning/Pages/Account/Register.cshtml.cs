using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using EventPlanning.Data;
using Microsoft.AspNetCore.Html;

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

            await _context.Users.AddAsync(User);
            await _context.SaveChangesAsync();
            return RedirectToPage("/Login");
        }
    }
}
