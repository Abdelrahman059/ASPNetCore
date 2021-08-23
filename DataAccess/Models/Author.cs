using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
   public class Author
    {
        [Required]
        [StringLength(64, MinimumLength = 5)]
        [Column(TypeName = "varchar(64)")]
        public string FullName { get; set; }
        public int Id { get; set; }

        [Required(ErrorMessage = "UserEmail is required")]
        [StringLength(64, MinimumLength = 5)]
        [Column(TypeName = "varchar(64)")]
        public string UserEmail { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [StringLength(32, MinimumLength = 5)]
        [Column(TypeName = "varchar(32)")]
        public string Password { get; set; }
        public Author()
        { }

    }
}
