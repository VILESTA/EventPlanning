using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace EventPlanning
{
    public class User
    {
        public int Id { get; set; }
        public IdentityRole Role { get; set; } = new IdentityRole("admin");

        [Required(ErrorMessage = "Не указан Email")]
        public string Email { get; set; } = "";
        public bool IsEmailConfirmed { get; set; } = false;

        [Required(ErrorMessage = "Не указан Пароль")]
        public string Password { get; set; } = "";

        [Required(ErrorMessage = "Не указано подтверждение пароля")]
        public string ConfirmPassword { get; set; } = ""; 
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string DisplayName
        {
            get
            {
                if(FirstName == null && LastName == null)
                {
                    return Email;
                }
                else
                {
                    return $"{FirstName} {LastName}";
                }
            }
        }
    }
}
