using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Sale.Shared.Entities
{
    public class Product
    {
        public int Id { get; set; }

        [Display(Name = "Name")]
        [MaxLength(50, ErrorMessage = "The {0} field must have a maximum of {1} characters.")]
        [Required(ErrorMessage = "The {0} field is required.")]
        public string Name { get; set; } = null!;

        [DataType(DataType.MultilineText)]
        [Display(Name = "Description")]
        [MaxLength(500, ErrorMessage = "The {0} field must have a maximum of {1} characters.")]
        public string Description { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        [Display(Name = "Price")]
        [Required(ErrorMessage = "The {0} field is required.")]
        public decimal Price { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Display(Name = "Inventory")]
        [Required(ErrorMessage = "The {0} field is required.")]
        public float Stock { get; set; }

        [Display(Name = "Categories")]

        public ICollection<ProductCategory>? productCategories { get; set; }

        public int ProductCategoriesNumber => productCategories==null ? 0 : productCategories.Count;

        [Display (Name ="Images")]

        public ICollection<ProductImage>? productImages { get; set; }
        [Display(Name = "Images")]
        public int ProductImagesNumber => productImages == null ? 0 : productImages.Count;

        public string MainImage => productImages == null ? string.Empty : productImages.FirstOrDefault()!.Image;
    }
}
