using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EventPlanning.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly EventPlanning.Data.ApplicationContext _context;

        public LoginModel(EventPlanning.Data.ApplicationContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public User User { get; set; } = default!;


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || _context.Users == null || User == null)
            {
                return Page();
            }
            if(_context.Users.First(p => p.Email == User.Email && p.Password == User.Password && p.IsEmailConfirmed) != null && _context.Users.Count() > 0)
            {
                return RedirectToPage("/Index");
            }
            return Page();
        }
    }
}