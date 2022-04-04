using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ViewModels
{
    public class LoginViewModel
    {
        /// <summary>
        /// User Email for login
        /// </summary>
        [Required(ErrorMessage = "User Name is required")]
        public string Username { get; set; }

        /// <summary>
        /// User Password
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        
        /// <summary>
        /// If user want us to remember his login or not
        /// </summary>
        public bool RememberMe { get; set; }
    }
}