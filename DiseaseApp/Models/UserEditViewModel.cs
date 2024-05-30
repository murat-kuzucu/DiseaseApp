using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DiseaseApp.Models
{
    public class UserEditViewModel
    {
        public int UserId { get; set; }
        
        [StringLength(16, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
        [Display(Name = "Username")]
        [RegularExpression(@"^[a-zA-Z0-9_.]+$", ErrorMessage = "The User Name can only contain letters, numbers, underscores, and periods.")]
        public string? UserName { get; set; }

        [StringLength(24, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        [Display(Name = "Name")]
        // Only allow letters (both uppercase and lowercase) and spaces
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "The Name can only contain letters and spaces.")]
         public string? Name { get; set; }
        
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [Display(Name = "Email")]
        [RegularExpression(@"^(?!.*@psyassistant\.com).*", ErrorMessage = "Email cannot contain '@psyassistant.com'.")]
        public string? Email { get; set; }

        [StringLength(24, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string? ConfirmPassword { get; set; }

        public string? UserRole { get;set;} = "User";

        public bool IsActive { get;set;}
    }
}