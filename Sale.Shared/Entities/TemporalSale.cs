using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Sale.Shared.Entities
{
    public class TemporalSale
    {
        public int Id { get; set; }
        public User? User { get; set; } 
        public string? userId { get; set; }
        public Product? Product { get; set; }
        public int productId { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Display(Name = "Amount")]
        [Required(ErrorMessage = "The {0} field is required.")]
        public float Quantity { get; set; }
        [DataType(DataType.MultilineText)]
        [Display(Name = "Comments")]
        public string? Remarks { get; set; }

        public decimal vlaue => Product == null ? 0 : Product.Price * (decimal)Quantity;
    }
}
