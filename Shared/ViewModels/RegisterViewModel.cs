using System.ComponentModel.DataAnnotations;

namespace Shared.ViewModels
{
    public class RegisterViewModel
    {
        /// <summary>
        /// User Email for register
        /// </summary>
        [Required(ErrorMessage = "User Name is required")]
        public string Username { get; set; }

        /// <summary>
        /// User Email for register
        /// </summary>
        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        /// <summary>
        /// User Email for register
        /// </summary>
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}