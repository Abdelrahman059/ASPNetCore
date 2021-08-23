using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class Post
    {
        public int Id { get; set; }

        [ForeignKey(nameof(Author))]
        public int AuthorId { get; set; }
        public Author author { get; set; }
      
        [Required]
        [StringLength(128, MinimumLength = 5)]
        [Column(TypeName = "varchar(128)")]
        public string Title { get; set; }
        [Required]
        [StringLength(128, MinimumLength = 5)]
        [Column(TypeName = "varchar(128)")]
        public string Subtitle { get; set; }
        public string ImageUrl { get; set; }
        [Column(TypeName = "varchar(32)")]
        public DateTime CreatedDate { get; set; }
        [Required]
        [MinLength(10)]
        public string Body { get; set; }

    }
}
