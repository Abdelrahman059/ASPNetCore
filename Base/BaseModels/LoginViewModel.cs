using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base
{
   public class LoginViewModel
    {
          
        [Required(ErrorMessage = "User Email is required")]
        [EmailAddress]
        public string UserEmail { get; set; }
        [DataType(DataType.Password)]

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        [Display(Name ="Remember Me")]
        public bool RememberMe  { get; set; }
    }
}

