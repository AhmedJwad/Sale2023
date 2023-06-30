using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Sale.Shared.Entities
{
   public class ProductImage
    {
        public int Id { get; set; }
        public Product Product { get; set; } = null!;
        public int ProductId { get; set; }
        [Display(Name = "Image")]
        public string Image { get; set; } = null!;

    }
}
