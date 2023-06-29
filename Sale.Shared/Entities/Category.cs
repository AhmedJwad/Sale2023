using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sale.Shared.Entities
{
    public class Category
    {
        public int Id { get; set; }

        [Display(Name = "Category")]
        [MaxLength(100, ErrorMessage = "The {0} field must have a maximum of {1} characters.")]
        [Required(ErrorMessage = "The {0} field is required.")]
        public string Name { get; set; } = null!;  
    }
}
