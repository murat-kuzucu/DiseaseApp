using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DiseaseApp.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(16, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
        [Display(Name = "Username")]
        [RegularExpression(@"^[a-zA-Z0-9_.]+$", ErrorMessage = "The User Name can only contain letters, numbers, underscores, and periods.")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(24, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        [Display(Name = "Name")]
        // Only allow letters (both uppercase and lowercase) and spaces
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "The Name can only contain letters and spaces.")]
         public string? Name { get; set; }
        
        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [Display(Name = "Email")]
        [RegularExpression(@"^(?!.*@psyassistant\.com).*", ErrorMessage = "Email cannot contain '@psyassistant.com'.")]
        public string? Email { get; set; }

        //TODO:User role can be added here, Researcher or Admin or User, etc.
        [Required]
        [StringLength(24, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string? Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string? ConfirmPassword { get; set; }

        public string UserRole { get;} = "User";

        public bool IsActive { get;} = true;

        

    }
}
